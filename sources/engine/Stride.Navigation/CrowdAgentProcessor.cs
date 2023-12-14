using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Engine;
using Stride.Games;
using Stride.Core;
using Stride.Core.Annotations;

namespace Stride.Navigation
{
    public class CrowdAgentProcessor : EntityProcessor<CrowdAgentComponent>
    {
        private CrowdSystem crowdSystem;

        protected override void OnSystemAdd()
        {
            // This is the same kind of entry point as used in PhysicsProcessor and BoundingBoxProcessor
            var gameSystems = Services.GetSafeServiceAs<IGameSystemCollection>();
            crowdSystem = gameSystems.OfType<CrowdSystem>().FirstOrDefault();
            if (crowdSystem == null)
            {
                crowdSystem = new CrowdSystem(Services);
                gameSystems.Add(crowdSystem);
            }
        }

        protected override void OnEntityComponentAdding(Entity entity, [NotNull] CrowdAgentComponent component, [NotNull] CrowdAgentComponent data)
        {
            base.OnEntityComponentAdding(entity, component, data);
        }

        protected override void OnEntityComponentRemoved(Entity entity, [NotNull] CrowdAgentComponent component, [NotNull] CrowdAgentComponent data)
        {
            base.OnEntityComponentRemoved(entity, component, data);
        }
    }
}
