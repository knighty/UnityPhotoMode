namespace PhotoMode
{
	public interface FlyCameraInputMode
	{
		void Process(FlyCameraInput input, ref FlyCameraControl control);
	}
}
