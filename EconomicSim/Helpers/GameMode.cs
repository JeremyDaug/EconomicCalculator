namespace EconomicSim.Helpers;

public enum GameMode
{
    /// <summary>
    /// No mode has been selected, and a mode should be selected.
    /// </summary>
    Null,
    /// <summary>
    /// Allows for unlimited editing and control, including broad stroke
    /// changes. Deletions still not supported
    /// </summary>
    God,
    /// <summary>
    /// Allows viewing and progressing, but no changes to data.
    /// </summary>
    Observer,
    /// <summary>
    /// Allows viewing, progressing, and modifying economic data
    /// such as prices, taxes, wages, and the like.
    /// </summary>
    InvisibleHand,
    /// <summary>
    /// Allows you to view and run one business in the system.
    /// May set prices, change jobs, process demands, and the like.
    /// </summary>
    Entrepreneur,
    /// <summary>
    /// Takes over a state, and the powers and limitations that entails.
    /// </summary>
    State,
    /// <summary>
    /// A Criminal enterprise, expected to break the law and create trouble.
    /// </summary>
    Criminal,
    /// <summary>
    /// Represent a group of people and their culture.
    /// </summary>
    Culture,
    /// <summary>
    /// Represents an ideology and those who support it.
    /// </summary>
    Ideology,
    /// <summary>
    /// Represents an institution, a portion of a larger government, but also
    /// partially independent from it.
    /// </summary>
    Institution,
    /// <summary>
    /// Represents a machine empire.
    /// </summary>
    Machine,
    /// <summary>
    /// Represents an ecology and a tree of life, or subtree within it.
    /// </summary>
    Ecology,
    /// <summary>
    /// Represents an organized ecology which has unified into
    /// a super-organism.
    /// </summary>
    Hivemind
}