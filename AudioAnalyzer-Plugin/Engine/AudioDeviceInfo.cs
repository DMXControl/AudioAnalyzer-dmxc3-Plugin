namespace AudioAnalyzer.Engine
{
    /// <summary>
    /// An audio device as the GUI needs to know it. Plain data on purpose - once analysis
    /// and GUI are separate processes this has to survive serialisation.
    /// </summary>
    public class AudioDeviceInfo
    {
        public AudioDeviceInfo(string id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>stable identifier, e.g. "WASAPI:{0.0.1.00000000}.{...}"</summary>
        public string Id { get; }

        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
