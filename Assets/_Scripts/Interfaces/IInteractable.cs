public interface IInteractable{
	public string InteractionPrompt {get;}
	public abstract bool Interact(PlayerInteract player);
}
