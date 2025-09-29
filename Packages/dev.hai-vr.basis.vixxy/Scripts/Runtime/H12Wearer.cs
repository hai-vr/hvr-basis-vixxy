using HVR.Basis.Comms.HVRUtility;
using LiteNetLib;

namespace Hai.Project12.VixxyBasisNet.Runtime
{
    internal class H12Wearer : I12Net
    {
        private readonly P12VixxyBasisNetworking _vixxyNet;

        public H12Wearer(P12VixxyBasisNetworking vixxyNet)
        {
            _vixxyNet = vixxyNet;
        }

        public void OnNetworkInitialized()
        {
            // If Non-Wearers loaded our avatar before the Wearer did, then the Wearer never received the request for a full snapshot.
            // Therefore, by default the Wearer submits a full snapshot upon network initialized.
            _vixxyNet.Wearer_SubmitFullSnapshot_ToAllNonWearers();
        }

        public void OnNetworkMessageReceived(H12AvatarContextualUser RemoteUser, byte[] unsafeBuffer, DeliveryMethod DeliveryMethod)
        {
            if (!RemoteUser.IsWearer)
            {
                ProceedWithDecoding(unsafeBuffer, RemoteUser);
            }
            else
            {
                HVRLogging.StateError("Wearer can't receive a message from wearer.");
            }
        }

        public void OnNetworkMessageServerReductionSystem(byte[] unsafeBuffer)
        {
            HVRLogging.StateError("Server reduction system message cannot be received by wearer.");
        }

        private void ProceedWithDecoding(byte[] unsafeBuffer, H12AvatarContextualUser remoteUser)
        {
            if (unsafeBuffer.Length == 0)
            {
                HVRLogging.ProtocolError("Buffer cannot be empty.");
                return;
            }

            byte packetId = unsafeBuffer[0];
            switch (packetId)
            {
                case I12Net.RequestState_NW_to_W:
                    if (unsafeBuffer.Length != 1)
                    {
                        HVRLogging.ProtocolError($"Buffer has incorrect length (expected 1, was {unsafeBuffer.Length}.");
                        return;
                    }

                    _vixxyNet.Wearer_SubmitFullSnapshotTo(remoteUser);
                    break;

                default:
                    HVRLogging.ProtocolError("Unknown packet ID.");
                    break;
            }
        }
    }
}
