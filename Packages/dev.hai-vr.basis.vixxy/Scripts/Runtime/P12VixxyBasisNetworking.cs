﻿using System;
using Basis.Scripts.BasisSdk;
using Basis.Scripts.Behaviour;
using HVR.Basis.Comms.HVRUtility;
using LiteNetLib;
using UnityEngine;

namespace HVR.Basis.Vixxy.Runtime
{
    public class P12VixxyBasisNetworking : BasisAvatarMonoBehaviour, I12VixxyNetworkable
    {
        private const DeliveryMethod MainMessageDeliveryMethod = DeliveryMethod.Sequenced;

        [SerializeField] private P12VixxyOrchestrator orchestrator;
        [SerializeField] private BasisAvatar avatar;

        private ushort _wearerId;
        private bool _isNetworkInitialized;
        private I12Net _relayLateInit;

        private void Awake()
        {
            orchestrator.OnNetworkDataUpdateRequired += OnNetworkDataUpdateRequired;

            if (avatar == null) avatar = GetComponentInParent<BasisAvatar>(true);
        }

        private void OnNetworkDataUpdateRequired()
        {
            if (!_isNetworkInitialized) return;

        }

        internal void Wearer_SubmitFullSnapshot_ToAllNonWearers()
        {
        }

        public virtual void OnNetworkReady(bool IsLocallyOwned)
        {
            _wearerId = avatar.LinkedPlayerID;
            if (_relayLateInit != null)
            {
                HVRLogging.ProtocolAccident("Received OnNetworkChange more than once in this object's lifetime, this is not normal.");
                return;
            }
            _relayLateInit = IsLocallyOwned ? new H12Wearer(this) : new H12NonWearer(this);
            _relayLateInit.OnNetworkInitialized();
        }

        public virtual void OnNetworkMessageReceived(ushort RemoteUser, byte[] unsafeBuffer, DeliveryMethod DeliveryMethod, bool IsADifferentAvatarLocally)
        {
            if (_relayLateInit != null) _relayLateInit.OnNetworkMessageReceived(User(RemoteUser), unsafeBuffer, DeliveryMethod);
            else HVRLogging.ProtocolAccident("Received OnNetworkMessageReceived before any OnNetworkChange was received.");
        }

        public virtual void OnNetworkMessageServerReductionSystem(byte[] unsafeBuffer, bool IsADifferentAvatarLocally)
        {
            if (_relayLateInit != null) _relayLateInit.OnNetworkMessageServerReductionSystem(unsafeBuffer);
            else HVRLogging.ProtocolAccident("Received OnNetworkMessageServerReductionSystem before any OnNetworkChange was received.");
        }

        public void SubmitReliable(byte[] buffer)
        {
            NetworkMessageSend(buffer, MainMessageDeliveryMethod);
        }

        private H12AvatarContextualUser User(ushort user)
        {
            return new H12AvatarContextualUser
            {
                User = user,
                IsWearer = user == _wearerId
            };
        }

        public void Wearer_SubmitFullSnapshotTo(H12AvatarContextualUser remoteUser)
        {
            throw new System.NotImplementedException();
        }

        public void NonWearer_ProcessFullSnapshot(object subBuffer)
        {
            throw new NotImplementedException();
        }

        public void RequireNetworked(string address, float defaultValue, P12VixxyNetDataUsage netDataUsage)
        {
        }
    }

    public struct H12AvatarContextualUser
    {
        public ushort User;
        public bool IsWearer;
    }

    internal interface I12Net
    {
        /// A Non-Wearer requests a full snapshot from the Wearer.
        internal const byte RequestState_NW_to_W = 0x01;
        /// The Wearer submits a full snapshot to a Non-Wearer.
        internal const byte SubmitFullSnapshot_W_to_NW = 0x02;
        /// The Wearer submits an incremental update to a Non-Wearer.
        internal const byte SubmitIncremental_W_to_NW = 0x03;
        /// The Wearer submits a piece of information that is not an incremental update to a Non-Wearer,
        /// but that information will cause the state to change from the perspective of that Non-Wearer.
        /// This can be, for example, information that pertains to a change in outfit, which would incur an implied change in the state,
        /// without needing to submit a change of the state itself.
        internal const byte SubmitEvent_W_to_NW = 0x04;

        void OnNetworkInitialized();
        void OnNetworkMessageReceived(H12AvatarContextualUser RemoteUser, byte[] unsafeBuffer, DeliveryMethod DeliveryMethod);
        void OnNetworkMessageServerReductionSystem(byte[] unsafeBuffer);
    }
}
