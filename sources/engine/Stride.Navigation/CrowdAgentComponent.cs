using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Collections;
using Stride.Engine;
using Stride.Engine.Design;
using Stride.Navigation.Processors;

namespace Stride.Navigation
{
    [DefaultEntityComponentProcessor(typeof(CrowdAgentProcessor), ExecutionMode = ExecutionMode.All)]

    public class CrowdAgentComponent : StartupScript
    {
        /// <summary>
        /// The navigation mesh group to use
        /// </summary>
        [DataMember(20)]
        [Display("Group")]
        public Guid NavigationGroup { get; set; }

        [DataMember(4)]
        [DataMemberRange(0, int.MaxValue)]
        public float maxAcceleration;              ///< Maximum allowed acceleration. [Limit: >= 0]

        [DataMember(5)]
        [DataMemberRange(0, int.MaxValue)]
        public float maxSpeed;                     ///< Maximum allowed speed. [Limit: >= 0]


        public override void Start()
        {
            base.Start();
            var crowdSystem = Game.GameSystems.OfType<CrowdSystem>().FirstOrDefault();
            // Wait for the dynamic navigation to be registered
            if (crowdSystem == null)
            {
                Game.GameSystems.CollectionChanged += GameSystemsOnCollectionChanged;
            }
            else
            {
                crowdSystem.AddAgent(this);
            }
        }

        private void GameSystemsOnCollectionChanged(object sender, TrackingCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var crowdSystem = e.Item as CrowdSystem;
                if (crowdSystem != null)
                {
                    crowdSystem.AddAgent(this);
                    // No longer need to listen to changes
                    Game.GameSystems.CollectionChanged -= GameSystemsOnCollectionChanged;
                }
            }
        }

        
    }
}
