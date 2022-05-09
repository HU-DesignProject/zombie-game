// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraWork.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in PUN Basics Tutorial to deal with the Camera work to follow the player
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

	/// <summary>
	/// Camera work. Follow a target
	/// </summary>
	public class KameraKontrol : MonoBehaviour
	{
        #region Private Fields

	    [Tooltip("The distance in the local x-z plane to the target")]
	    [SerializeField]
	    private float distance = 7.0f;
	    
	    [Tooltip("The height we want the camera to be above the target")]
	    [SerializeField]
	    private float height = 3.0f;
	    
	    [Tooltip("Allow the camera to be offseted vertically from the target, for example giving more view of the sceneray and less ground.")]
	    [SerializeField]
	    private Vector3 centerOffset = Vector3.zero;

	    [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
	    [SerializeField]
	    private bool followOnStart = false;

	    [Tooltip("The Smoothing for the camera to follow the target")]
	    [SerializeField]
	    private float smoothSpeed = 0.125f;

        // cached transform of the target
        Transform cameraTransform;

		// maintain a flag internally to reconnect if target is lost or camera is switched
		bool isFollowing;
		
		// Cache for camera offset
		Vector3 cameraOffset = Vector3.zero;

		public Transform hedef;

		public Vector3 hedefMesafe;
		[SerializeField]
		private float fareHassasiyeti;
		float fareX, fareY;

		Vector3 objRot;
		public Transform karakterVucut;

		KarakterKontrol karakterHp;
		
		
        #endregion

        #region MonoBehaviour Callbacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase
        /// </summary>
        void Start()
		{
			
			karakterHp = GameObject.Find("SWAT").GetComponent<KarakterKontrol>();
			hedef=karakterHp.target;
			// Start following the target if wanted.
			if (followOnStart)
			{
				OnStartFollowing();
			}
		}


		void LateUpdate()
		{
			// The transform target may not destroy on level load, 
			// so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens
			if (cameraTransform == null && isFollowing)
			{
				OnStartFollowing();
			}

			// only follow is explicitly declared
			if (isFollowing) {
				Follow ();
			}

			this.transform.position = Vector3.Lerp(this.transform.position, hedef.position + hedefMesafe, Time.deltaTime * 10);
            fareX += Input.GetAxis("Mouse X") * fareHassasiyeti;
            fareY += Input.GetAxis("Mouse Y") * fareHassasiyeti;
            if (fareY >= 25)
            {
                fareY = 25;
            }
            if (fareY <= -40)
            {
                fareY = -40;
            }
            this.transform.eulerAngles = new Vector3(fareY, fareX, 0);
            hedef.transform.eulerAngles = new Vector3(0, fareX, 0);

            Vector3 gecici = this.transform.eulerAngles;
            gecici = this.transform.eulerAngles;
            gecici.z = 0;
            gecici.y = this.transform.localEulerAngles.y;
            gecici.x = this.transform.localEulerAngles.x + 10;
            objRot = gecici;
            karakterVucut.transform.eulerAngles = objRot;

		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Raises the start following event. 
		/// Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
		/// </summary>
		public void OnStartFollowing()
		{	      
			cameraTransform = Camera.main.transform;
			isFollowing = true;
			// we don't smooth anything, we go straight to the right camera shot
			Cut();
		}
		
		#endregion

		#region Private Methods

		/// <summary>
		/// Follow the target smoothly
		/// </summary>
		void Follow()
		{
			cameraOffset.z = -distance;
			cameraOffset.y = height;
			
		    cameraTransform.position = Vector3.Lerp(cameraTransform.position, this.transform.position +this.transform.TransformVector(cameraOffset), smoothSpeed*Time.deltaTime);

		    cameraTransform.LookAt(this.transform.position + centerOffset);
		    
	    }

	   
		void Cut()
		{
			cameraOffset.z = -distance;
			cameraOffset.y = height;

			cameraTransform.position = this.transform.position + this.transform.TransformVector(cameraOffset);

			cameraTransform.LookAt(this.transform.position + centerOffset);
		}
		#endregion

        
	}
