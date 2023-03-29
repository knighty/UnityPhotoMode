using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI
{
	abstract public class Command
	{
		abstract public string Name { get; }

		abstract public void Execute();
		abstract public void Undo();

		public override string ToString()
		{
			return Name;
		}
	}

	public interface MergeableCommand<T>
	{
		public bool CanMerge(T command);
		public void Merge(T command);
	}

	public class SetPhotoModeSettingValue<T> : Command, MergeableCommand<SetPhotoModeSettingValue<T>>
	{
		public PhotoModeSetting<T> setting;
		public T value;
		public T oldValue;

		public int MergeID => setting.GetHashCode();

		public SetPhotoModeSettingValue(PhotoModeSetting<T> setting, T value, T oldValue)
		{
			this.setting = setting;
			this.value = value;
			this.oldValue = oldValue;
		}

		public override string Name => "Set PhotoMode Setting Value";

		public override void Execute() => setting.Value = value;

		public override void Undo() => setting.Value = oldValue;

		public void Merge(SetPhotoModeSettingValue<T> command) => value = command.value;

		public bool CanMerge(SetPhotoModeSettingValue<T> command) => command.setting == setting;
	}

	public class CommandList
	{
		private const int MAX_SIZE = 100;
		private Stack<Command> undoStack;
		private Stack<Command> redoStack;

		public Action<Command> OnAdd;
		public Action<Command> OnUndo;
		public Action<Command> OnRedo;
		public Action OnModify;

		private static CommandList instance;
		public static CommandList Instance
		{
			get => instance ??= new CommandList();
		}

		private CommandList()
		{
			undoStack = new Stack<Command>(MAX_SIZE);
			redoStack = new Stack<Command>();
		}

		public Command Add<T>(T command) where T : Command
		{
			if (undoStack.Count > 0)
			{
				Command previous = undoStack.Peek();
				if (previous is MergeableCommand<T> mergeablePrevious && command is MergeableCommand<T> mergableCommand && mergeablePrevious.CanMerge(command))
				{
					mergeablePrevious.Merge(command);
					command.Execute();
					return previous;
				}
			}

			undoStack.Push(command);
			command.Execute();
			redoStack.Clear();
			OnAdd?.Invoke(command);
			OnModify?.Invoke();
			return command;
		}

		public Command Undo()
		{
			if (undoStack.Count == 0)
				return null;
			Command command = undoStack.Pop();
			command.Undo();
			redoStack.Push(command);
			OnUndo?.Invoke(command);
			OnModify?.Invoke();
			return command;
		}

		public Command Redo()
		{
			if (redoStack.Count == 0)
				return null;
			Command command = redoStack.Pop();
			command.Execute();
			undoStack.Push(command);
			OnRedo?.Invoke(command);
			OnModify?.Invoke();
			return command;
		}

		public override string ToString()
		{
			return
				$"[Undo Stack {undoStack.Count})]\n" +
				undoStack.Aggregate("", (str, command) => str + "- " + command.Name + "\n") +
				$"[Redo Stack {redoStack.Count})]\n" +
				redoStack.Aggregate("", (str, command) => str + "- " + command.Name + "\n");
		}
	}


	public partial class PhotoModeSettingsEditor : MonoBehaviour, ISerializationCallbackReceiver
	{
		[Serializable]
		class EditorPrefabFactory
		{
			[SerializeField] public string propertyName;
			[SerializeField] public SettingEditor editorPrefab;

			public SettingEditor Create(PropertyInfo property, PhotoModeSetting setting)
			{
				SettingEditor editor = Instantiate(editorPrefab);
				editor.PropertyInfo = property;
				editor.Setting = setting;
				return editor.GetComponent<SettingEditor>();
			}
		}

		[SerializeField] PhotoModeSettings settings;
		[SerializeField] GameObject propertiesList;
		[SerializeField] SettingEditor floatPrefab;
		[SerializeField] List<EditorPrefabFactory> editorPrefabFactories;
		[SerializeField] RectTransform tabBar;
		[SerializeField] TabButton tabPrefab;
		[SerializeField] RectTransform tabSelectedRect;
		[SerializeField] Button profilesButton;
		[SerializeField] ProfilesWindow profilesWindow;
		[SerializeField] Button optionsButton;
		[SerializeField] OptionsWindow optionsWindow;

		[SerializeField] List<string> serializeEditorPrefabFactoriesName;
		[SerializeField] List<SettingEditor> serializeEditorPrefabFactoriesPrefab;

		TabCollection tabCollection = new TabCollection();
		Coroutine tabCoroutine = null;

		public PhotoModeSettings Settings
		{
			get => settings;
			set
			{
				profilesWindow.Settings = value;
				settings = value;
				AddTabs(settings);
			}
		}

		protected void AddTabs(PhotoModeSettings settings)
		{
			tabCollection.AddTabs(
				settings.GetType().GetProperties()
					.Select(property => property.GetCustomAttribute<CategoryAttribute>())
					.Where(attr => attr != null)
					.Select(attr => attr.category)
					.Distinct()
			);

			foreach (TabCollection.Tab tab in tabCollection.Tabs)
			{
				TabButton tabButton = Instantiate(tabPrefab, tabBar.transform);
				tabButton.Text = tab.Name;
				tabButton.OnClick += tabButton => SelectTab(tab);
			}

			SelectTab(tabCollection.FindTabByName("Camera"));
		}

		protected void SelectTab(TabCollection.Tab tab)
		{
			if (tabCoroutine != null)
				StopCoroutine(tabCoroutine);

			tabCoroutine = StartCoroutine(Utils.TweenTime(tabSelectedRect.anchorMin.x, tab.id / (float)tabCollection.Count, 0.5f, x =>
			{
				tabSelectedRect.anchorMin = new Vector2(x, 0);
				tabSelectedRect.anchorMax = new Vector2(x + 1.0f / (float)tabCollection.Count, 1);
			}, Utils.EaseOutQuart));

			profilesWindow.Category = tab.Name;
			AddSettings(settings, tab.Name);
		}

		protected void ClearSettings()
		{
			int childs = propertiesList.transform.childCount;
			for (int i = childs - 1; i >= 0; i--)
			{
				Destroy(propertiesList.transform.GetChild(i).gameObject);
			}
		}

		protected void AddSettings(PhotoModeSettings settings, string category = "")
		{
			ClearSettings();
			Dictionary<string, EditorPrefabFactory> factories = new Dictionary<string, EditorPrefabFactory>();
			foreach (var factory in editorPrefabFactories)
			{
				factories.Add(factory.propertyName, factory);
			}

			foreach (var property in settings.GetType().GetProperties())
			{
				if (property.GetCustomAttribute<CategoryAttribute>() is var categoryAttribute && categoryAttribute != null)
				{
					if (categoryAttribute.category != category)
						continue;
				}

				if (typeof(PhotoModeSetting).IsAssignableFrom(property.PropertyType))//property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(PhotoModeSetting<>))
				{
					SettingEditor editor = null;
					PhotoModeSetting setting = (PhotoModeSetting)property.GetValue(settings);
					if (factories.TryGetValue(property.Name, out EditorPrefabFactory factory))
					{
						editor = factory.Create(property, setting);
					}
					else
					{
						switch (setting)
						{
							case PhotoModeSettingFloat floatSetting:
								{
									editor = Instantiate(floatPrefab);
									editor.PropertyInfo = property;
									editor.Setting = floatSetting;

									break;
								}
						}
					}

					if (editor != null)
					{
						if (property.GetCustomAttribute<PhotoModeSettingAttribute>() is PhotoModeSettingAttribute attr && attr != null)
						{
							editor.Label = attr.name;
						}
						else
						{
							editor.Label = property.Name;// ObjectNames.NicifyVariableName(property.Name);
						}
						editor.transform.SetParent(propertiesList.transform);
					}
					else
					{
						Debug.Log($"Photo mode settings could not make an editor for \"{property.Name}\"");
					}
				}
			}
		}

		void Awake()
		{
			if (settings != null)
			{
				AddTabs(settings);
			}

			profilesButton.onClick.AddListener(OnClickProfilesButton);
			optionsButton.onClick.AddListener(OnClickOptionsButton);
		}

		private void OnClickProfilesButton()
		{
			profilesWindow.gameObject.SetActive(!profilesWindow.gameObject.activeSelf);
		}

		private void OnClickOptionsButton()
		{
			optionsWindow.gameObject.SetActive(!optionsWindow.gameObject.activeSelf);
		}

		private void Update()
		{
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
			{
				CommandList.Instance.Undo();
			}
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y))
			{
				CommandList.Instance.Redo();
			}
		}

		public void OnBeforeSerialize()
		{
			if (editorPrefabFactories != null)
			{
				serializeEditorPrefabFactoriesPrefab = editorPrefabFactories.Select(factory => factory.editorPrefab).ToList();
				serializeEditorPrefabFactoriesName = editorPrefabFactories.Select(factory => factory.propertyName).ToList();
			}
		}

		public void OnAfterDeserialize()
		{
			if (Application.isEditor)
				return;

			editorPrefabFactories = serializeEditorPrefabFactoriesName.Zip(serializeEditorPrefabFactoriesPrefab, (name, prefab) => new EditorPrefabFactory() { propertyName = name, editorPrefab = prefab }).ToList();
		}
	}
}