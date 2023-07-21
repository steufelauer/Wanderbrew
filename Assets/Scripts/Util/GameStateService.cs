
public interface IGameStateService
{
    GameState CurrentGameState { get; }
    void ChangeState(GameState newState);
}

public class GameStateService : IGameStateService
{
    public GameState CurrentGameState { get { return currentGameState; } }
    private GameState currentGameState = GameState.Main;
    public void ChangeState(GameState newState)
    {
        this.currentGameState = newState;
    }
}

public enum GameState
{
    None,
    Main,
    Minigame
}