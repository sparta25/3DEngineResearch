namespace TestFramework
{
    public interface ITestable
    {
        /// <summary>
        ///     Is called by the testing framework to set the scene up.
        /// </summary>
        void CreateScene();

        /// <summary>
        ///     Runs a test. Is called repeatedly by the framework.
        ///     Implementation is expected to render the scene and perform preparations for the next run (e.g. rotate the scene).
        /// </summary>
        void Render();
    }
}