namespace ValleyNet.Entity.Sync
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Networking;
    using ValleyNet.Network;


    [RequireComponent(typeof(NetworkIdentity))]
    public class NetSyncTransform : MonoBehaviour
    {
        /* SAMPLE PARAMETERS */
        [SerializeField]
        [Tooltip("Transform we want to sync. No assignment == gameObject.Transform")]
        private Transform _syncTrans;
        [SerializeField]
        [Tooltip("Updates sent/recv'd per second. Cannot be greater than fixedUpdateRate. 0 == Use Server Tick Rate")]
        private int _sampleRate = 0;
        public bool isSampling = false;
        
        /* SAMPLE FRAME DATA */
        private Vector3 _sampledPos;        // last position we've sampled
        private Quaternion _sampledRot;     // last rotation we've sampled
        private int _sampleSeqNum = 0;      // sequential id of sample frame
        private NetworkIdentity _networkId;  // which network'd obj does this sample belong to?
        
        /* GETTERS & SETTERS */
        public int netId                {get{return (int)_networkId.netId.Value;}}
        public Vector3 sampledPos       {get{return _sampledPos;}}
        public Quaternion sampledRot    {get{return _sampledRot;}}
        public int sampleSeqNum         {get{return _sampleSeqNum;}}
        public int sampleRate           {get{return _sampleRate;}}
        

        void Start()
        {
            // if custom sample rate has been
            if(_sampleRate > 0 && (1/_sampleRate) < Time.fixedDeltaTime)
            {
                IEnumerator _sampleData = SampleData();
                StartCoroutine(_sampleData);
            }

            if(_syncTrans == null)
            {
                _syncTrans = (Transform)gameObject.GetComponent<Transform>(); // Auto pair to 
            }
            _networkId = GetComponent<NetworkIdentity>();

            if(isSampling)
            {
                StartSampling();
            }
        }


        public void StartSampling()
        {
            UpdateSample(); // Initially setup sample
            isSampling = true;
        }


        public void SetSamplingState(bool isSampling)
        {
            this.isSampling = isSampling;
        }


        void FixedUpdate()
        {
            if(_sampleRate < 1 && isSampling)
            {
                UpdateSample();
            }
        }


        protected IEnumerator SampleData()
        {
            while(isSampling)
            {
                UpdateSample();
                yield return new WaitForSeconds(_sampleRate);
            }
        }

        protected void UpdateSample()
        {
            _sampledPos = _syncTrans.position;
            _sampledRot = _syncTrans.rotation;
            _sampleSeqNum++;

            //NetClientTransmitter.SendOnMain();
        }

    }
}