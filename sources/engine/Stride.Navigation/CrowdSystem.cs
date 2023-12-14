using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DXGI;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;
using Stride.Games;
using Stride.Rendering.Lights;

namespace Stride.Navigation
{
    public class CrowdSystem : GameSystem
    {
        /// <summary>
        /// A list of guids to traverse the GroupIdToCrowd dictionary every frame without generating garbage
        /// </summary>
        private List<Guid> groupIds;
        private Dictionary<Guid, Crowd> GroupIdToCrowd;

        public CrowdSystem(IServiceRegistry registry) : base(registry)
        {
        }

        public override void Initialize()
        {
            InitializeSettingsFromGameSettings(GetNavigationSettings());
        }

        private void InitializeSettingsFromGameSettings(NavigationSettings navigationSettings)
        {
            groupIds = new List<Guid>(navigationSettings.Groups.Count);
            GroupIdToCrowd = new Dictionary<Guid, Crowd>(navigationSettings.Groups.Count);
        }


        public void AddAgent(CrowdAgentComponent crowdAgent)
        {
            if (!GroupIdToCrowd.ContainsKey(crowdAgent.NavigationGroup))
            {
                var newCrowd = new Crowd();
                InitializeCrowd(newCrowd, crowdAgent.NavigationGroup);
                groupIds.Add(crowdAgent.NavigationGroup);
                GroupIdToCrowd.Add(crowdAgent.NavigationGroup, newCrowd);
            }

        }

        private void InitializeCrowd(Crowd crowd, Guid groupId)
        {
            
        }

        private NavigationSettings GetNavigationSettings()
        {
            var settings = Services.GetService<IGameSettingsService>()?.Settings;
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            // Initialize build settings from game settings
            return settings.Configurations.Get<NavigationSettings>();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
        }
    }
}
