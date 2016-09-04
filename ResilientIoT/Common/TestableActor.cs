using Microsoft.ServiceFabric.Actors.Runtime;

namespace Common
{
    public abstract class TestableActor : Actor
    {
        private IActorStateManager stateManager;

        public new IActorStateManager StateManager
        {
            get
            {
                return this.stateManager ?? base.StateManager;
            }

            set
            {
                this.stateManager = value;
            }
        }
    }
}
