// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerUI.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in PUN Basics Tutorial to deal with the networked player instance UI display tha follows a given player to show its health and name
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


	/// <summary>
	/// Player UI. Constraint the UI to follow a PlayerManager GameObject in the world,
	/// Affect a slider and text to display Player's name and health
	/// </summary>
	public class PlayerUI : MonoBehaviour
    {
        #region Private Fields

		private float health;
		private float lerpTimer;
		public float maxHealth=100f;
		public float chipSpeed=2f;
		public Image frontHealthBar;
		public Image backHealthBar;

		[Header ("Damage Screen")]
		public Color damageColor;
		public Image damageImage;
		float colorSmoothing=6f;
		bool isTakingDamage=false;

		GameObject targetPlayer;

	    [Tooltip("Pixel offset from the player target")]
        [SerializeField]
        private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

	    [Tooltip("UI Text to display Player's Name")]
	    [SerializeField]
	    private Text playerNameText;

		[Tooltip("UI Text to display Player's Bullet Count")]
	    [SerializeField]
	    private Text bulletCount;

	    [Tooltip("UI Slider to display Player's Health")]
	    [SerializeField]
	    private Slider playerHealthSlider;

        KarakterKontrol target;

		float characterControllerHeight;

		Transform targetTransform;

		Renderer targetRenderer;

	    CanvasGroup _canvasGroup;
	    
		Vector3 targetPosition;
		#endregion

		#region MonoBehaviour Messages
		
		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during early initialization phase
		/// </summary>
		void Awake()
		{

			
			targetPlayer = GameObject.Find("SWAT");
			health=targetPlayer.GetComponent<KarakterKontrol>().GetPlayerHealth();
			damageImage.enabled=false;
			
 			Debug.Log("playerUIdayım");
			_canvasGroup = this.GetComponent<CanvasGroup>();
			
			this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
			
           
		}

		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity on every frame.
		/// update the health slider to reflect the Player's health
		/// </summary>
		void Update()
		{
				// Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
			if (target == null) {
				Destroy(this.gameObject);
				return;
			}


			// Reflect the Player Health
			if (playerHealthSlider != null) {
				playerHealthSlider.value = target.playerHealth;
			}

			if (bulletCount.text != null) {
				bulletCount.text = this.target.photonView.Owner.BulletCount.ToString();
			}

			health=Mathf.Clamp(health,0,maxHealth);

			UpdateHealthUI();
			if(Input.GetKeyDown(KeyCode.N)){
				TakeDamage(Random.Range(5,10));
			}
			if(Input.GetKeyDown(KeyCode.M)){
				RestoreHealth(Random.Range(5,10));
			}

			if(isTakingDamage){
				damageImage.enabled=true;
				damageImage.color=damageColor;

			}
			else{
				damageImage.color=Color.Lerp(damageImage.color,Color.clear,colorSmoothing*Time.deltaTime*Time.deltaTime);

			}

			isTakingDamage=false;
			
			
		}

		/// <summary>
		/// MonoBehaviour method called after all Update functions have been called. This is useful to order script execution.
		/// In our case since we are following a moving GameObject, we need to proceed after the player was moved during a particular frame.
		/// </summary>
		void LateUpdate () {

			// Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
			if (targetRenderer!=null)
			{
				this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
			}
			
			// #Critical
			// Follow the Target GameObject on screen.
			if (targetTransform!=null)
			{
				targetPosition = targetTransform.position;
				targetPosition.y += characterControllerHeight;
				
				this.transform.position = Camera.main.WorldToScreenPoint (targetPosition) + screenOffset;
			}

		}




		#endregion

		#region Public Methods

		/// <summary>
		/// Assigns a Player Target to Follow and represent.
		/// </summary>
		/// <param name="target">Target.</param>
		public void SetTarget(KarakterKontrol _target){

			if (_target == null) {
				Debug.LogError("<Color=Red><b>Missing</b></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
				return;
			}

			// Cache references for efficiency because we are going to reuse them.
			this.target = _target;
            targetTransform = this.target.GetComponent<Transform>();
            targetRenderer = this.target.GetComponentInChildren<Renderer>();


            CharacterController _characterController = this.target.GetComponent<CharacterController> ();

			// Get data from the Player that won't change during the lifetime of this Component
			if (_characterController != null){
				characterControllerHeight = _characterController.height;
			}

			if (playerNameText != null) {
                playerNameText.text = this.target.photonView.Owner.NickName;
				
			}
		}

		public void UpdateHealthUI(){
			Debug.Log(health);
			float fillF=frontHealthBar.fillAmount;
			float fillB=backHealthBar.fillAmount;
			float hFraction=health/maxHealth;

			if(fillB>hFraction){
				frontHealthBar.fillAmount=hFraction;
				backHealthBar.color=Color.red;
				lerpTimer+=Time.deltaTime;
				float percentComplete =lerpTimer/chipSpeed;
				percentComplete=percentComplete*percentComplete;
				backHealthBar.fillAmount=Mathf.Lerp(fillB,hFraction,percentComplete);
			}

			if(fillF<hFraction){
				backHealthBar.color=Color.green;
				backHealthBar.fillAmount=hFraction;
				lerpTimer+=Time.deltaTime;
				float percentComplete =lerpTimer/chipSpeed;
				percentComplete=percentComplete*percentComplete;
				frontHealthBar.fillAmount=Mathf.Lerp(fillF,backHealthBar.fillAmount,percentComplete);
			}
		}

		public void TakeDamage(float damage){
			isTakingDamage=true;
			Debug.Log(damage);
			health-=damage;
			lerpTimer=0f;
		}

		public void RestoreHealth(float healAmount){
			health+=healAmount;
			lerpTimer=0f;
		}

		#endregion

	
}