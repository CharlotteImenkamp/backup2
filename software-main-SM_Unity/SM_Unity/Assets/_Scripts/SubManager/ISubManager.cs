
/// <summary>
/// Interface, which is implemeted in all submanagers. 
/// A submanager can be added and its methods are called in game, if its added to the gamemanager.
/// </summary>
public class SubManager
{
    public virtual void Reset() { }

    /// <summary>
    /// Check, if any task needs to be done at new game state
    /// </summary>
    /// <param name="newState"></param>
    public virtual void OnGameStateEntered(string newState) { }

    /// <summary>
    /// Check, if any task needs to be done before next game state
    /// </summary>
    /// <param name="oldState"></param>
    public virtual void OnGameStateLeft(string oldState) { }
}
