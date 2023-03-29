namespace PhotoMode.UI.Overlays
{
	public interface CompositionOverlay
	{
		public string Name { get; }
		public bool Enabled { set; get; }
	}
}
