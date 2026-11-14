public interface IInteractable
{
    string Prompt { get; }

    // Checked on the client (to show the prompt) AND on the server (to stay safe).
    bool CanInteract(PlayerInteractor who);

    // Runs on the SERVER only, after the server has validated the request.
    void ServerInteract(PlayerInteractor who);
}
