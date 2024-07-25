using Fusion;


enum PlayerButtons
{
	Fire = 0,
}

public struct PlayerInput : INetworkInput
{
	public float HorizontalInput;
	public float VerticalInput;
	public NetworkButtons Buttons;
}