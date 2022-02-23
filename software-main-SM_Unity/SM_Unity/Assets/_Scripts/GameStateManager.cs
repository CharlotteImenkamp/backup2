using System;
using UnityEngine;


/// <summary>
/// Singelton class
/// tasks: change gameStates, which are no data states
/// </summary>
public class GameStateManager : MonoBehaviour
{
    #region Instance

    private static GameStateManager instance = null;

    public static GameStateManager Instance { get => instance; }

    #endregion

    #region Private Fields

    private static StateMachine gameStateMachine = new StateMachine();

    #endregion Private Fields

    #region MonoBehaviour Functions

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            Debug.LogError("Instance of GameStateManager destroyed.");
        }
        else
            instance = this;
    }

    /// <summary>
    /// Initialize parameters if neccessary 
    /// </summary>
    private void Start()
    {
        ResetToDefault();
    }

    #endregion MonoBehaviour Functions

    #region Button Functions

    /// <summary>
    /// Starts game with test run. Changes state machine either to "PriceTest" or "LocationTest" depending on the input parameter. 
    /// Called at "UserButton" event of GameManager
    /// </summary>
    /// <param name="gameType">Game type to change state to.</param>
    public void StartTestRun(GameType gameType)
    {
        // Change game state
        if (gameType == GameType.Prices)
            gameStateMachine.ChangeState(new PriceTest());
        else if (gameType == GameType.Locations)
            gameStateMachine.ChangeState(new LocationTest());
        else
            throw new ArgumentException("GameStateManager::StartTestRun no valid GameType {0}", gameType.ToString());

        // Assign next function to userButton event
        GameManager.Instance.OnUserButtonClicked.RemoveAllListeners();
        GameManager.Instance.OnUserButtonClicked.AddListener(() => StartGame(GameManager.Instance.GameType));
    }

    /// <summary>
    /// Starts game after test run. Changes state machine either to "PriceEstimation" or "LocationEstimation" depending on the input parameter. 
    /// Starts the data logging in DataManager.
    /// Called at "UserButton" event of GameManager.
    /// </summary>
    /// <param name="gameType">GameType to change state to.</param>
    public void StartGame(GameType gameType)
    {
        // change type
        if (gameType == GameType.Prices)
        {
            gameStateMachine.ChangeState(new PriceEstimation());
            DataManager.Instance.StartDataLogging();
        }
        else if (gameType == GameType.Locations)
        {
            gameStateMachine.ChangeState(new LocationEstimation());
            DataManager.Instance.StartDataLogging();
        }
        else
            throw new ArgumentException("GameStateManager::StartTestRun no valid GameType.");

        // Assign next function to userButton event
        GameManager.Instance.OnUserButtonClicked.RemoveAllListeners();
        GameManager.Instance.OnUserButtonClicked.AddListener(() => EndGame(GameManager.Instance.GameType));
    }

    /// <summary>
    /// Ends game after estimation phase is over. Changes either to "Pause" or "End" depending on input parameter.
    /// </summary>
    /// <param name="gameType">Current game type.</param>
    public void EndGame(GameType gameType)
    {
        if (gameType == GameType.Prices)
        {
            // Stop logging before state change to get the objects in the scene
            DataManager.Instance.StopDataLogging();

            // Change state
            gameStateMachine.ChangeState(new Pause());
        }
        else if (gameType == GameType.Locations)
        {
            // Stop logging before state change to get the objects in the scene
            DataManager.Instance.StopDataLogging();

            // Change state
            gameStateMachine.ChangeState(new End());
        }
        else
            throw new ArgumentException("GameStateManager::EndGame no valid GameType.");

        // remove from event
        GameManager.Instance.OnUserButtonClicked.RemoveAllListeners();
    }

    #endregion Button Functions

    #region Public Functions

    /// <summary>
    /// Changes state to start state and removes event listeners.
    /// </summary>
    public void ResetToDefault()
    {
        // Events
        GameManager.Instance.OnUserButtonClicked.RemoveAllListeners();

        // game states
        gameStateMachine.ChangeState(new Initialization());
        gameStateMachine.ChangeState(new SettingsMenu());
    }

    #endregion Public Functions
}


