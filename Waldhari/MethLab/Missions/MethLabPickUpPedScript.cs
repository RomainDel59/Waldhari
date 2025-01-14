using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.UI;

namespace Waldhari.MethLab.Missions
{
    public class MethLabPickUpPedScript : GenericPickUpPedMissionScript
    {
        public MethLabPickUpPedScript() 
            : base("MethLabPickUpPedScript", "methlab_pickupped_success") { }


        protected override WPosition Destination => MethLabHelper.Positions.Parking;
        protected override void ShowStartedMessage()
        {
            NotificationHelper.ShowFromRon("methlab_pickupped_started");
        }
        protected override string PedMessageKey => "chemist";
        protected override string FailPedDeadMessageKey => "methlab_pickupped_fail_dead";
        protected override string RendezvousMessageKey => "methlab_pickupped_step_rendezvous";
        protected override string WaitMessageKey => "methlab_pickupped_step_wait";
        protected override string DriveMessageKey => "methlab_pickupped_step_drive";
        //(move_m@generic)MP_M_Meth_01
        protected override PedHash PedHash => (PedHash)3988008767;
        protected override string DestinationMessageKey => "methlab_parking";
    }
}