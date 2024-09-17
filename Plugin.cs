namespace Size
{
    using Exiled.API.Features;
    using System;
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public override string Author => "ZurnaSever";

        public override string Name => "Size";

        public override string Prefix => "Size";

        public override Version RequiredExiledVersion { get; } = new Version(8, 11, 0);

        public override Version Version { get; } = new Version(1, 0, 0);

        public override void OnEnabled()
        {
            Instance = this;
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Instance = null;
            base.OnDisabled();
        }
    }
}
