using HVR.Basis.Comms;

namespace HVR.Basis.Vixxy.Runtime
{
    public class H12ActuatorRegistrationToken
    {
        public string registeredAddress;
        public int registeredIddress;
        public AcquisitionService.AddressUpdated registeredCallback;
        public I12VixxyActuator registeredActuator;
    }
}
