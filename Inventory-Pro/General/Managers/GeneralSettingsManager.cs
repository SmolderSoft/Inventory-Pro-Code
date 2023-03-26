#pragma warning disable CS0618 // Assert.raiseExceptions is obsolete
using UnityEngine.Assertions;

namespace Devdog.General
{
    public partial class GeneralSettingsManager : ManagerBase<GeneralSettingsManager>
    {
        [Required]
        public GeneralSettings settings;


        protected override void Awake()
        {
            base.Awake();

            settings.defaultCursor.Enable();
            Assert.raiseExceptions = settings.useExceptionsForAssertions;
            DevdogLogger.minimaLog = settings.minimalLogType;
        }
    }
}
#pragma warning restore CS0618 // Assert.raiseExceptions is obsolete
