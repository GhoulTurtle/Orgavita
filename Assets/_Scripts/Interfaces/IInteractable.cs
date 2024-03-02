public interface IInteractable{
	public abstract string InteractionPrompt {get;}
	public abstract bool Interact(PlayerInteract player);
}
