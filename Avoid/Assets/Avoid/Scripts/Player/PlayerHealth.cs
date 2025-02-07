using UnityEngine;
using System.Collections;

namespace Avoidance
{
	/// <summary>
	/// Maintains players health. Responsible for spawning damage effects and playing hit audio.
	/// </summary>
	public class PlayerHealth : MonoBehaviour 
	{
		/// <summary>
		/// The effect to spawn on hit.
		/// </summary>
		public GameObject playerDamageEffect;

		/// <summary>
		/// The number of hits the player can take.
		/// </summary>
		public int hitPoints = 10;

		/// <summary>
		/// Clips to play on hit. A random clip is selected.
		/// </summary>
        public AudioClip[] onHitAudioClips;

		private float _currentScale;
		private float _scaleMultiplier;
		private Vector2 _initialScale;
		private Vector2 _minimumScale;
		private bool _isDead;

		void Start()
		{
			_isDead = false;
			_initialScale = transform.localScale;
			_minimumScale = _initialScale * 0.6f;

		}

		/// <summary>
		/// Applies damage, spawns damage effect, plays audio, and scales player down.
		/// </summary>
		public void ApplyDamage()
		{
			if (!_isDead) 
			{
                if(onHitAudioClips != null && onHitAudioClips.Length > 0)
                {
                    MusicAudioPlayer.instance.PlayOneShot(onHitAudioClips[Random.Range(0, onHitAudioClips.Length)]);
                }

				SpawnDamageEffect ();
				ScaleDown ();
			}
		}

		void OnEnable()
		{
			_currentScale = 0f;
			_scaleMultiplier = 1f / hitPoints;
		}

		private void SpawnDamageEffect()
		{
			var deathEffect = ObjectPool.instance.GetObjectForType (playerDamageEffect.name, false);
			deathEffect.transform.position = transform.position;
			deathEffect.SetActive (true);
		}

		private void ScaleDown()
		{
			_currentScale += _scaleMultiplier;

			transform.localScale = Vector2.Lerp (_initialScale, _minimumScale, _currentScale);

			if(_currentScale >= 1f)
			{
				if (GameStateController.instance.isCancelAd)
				{
					OnDead();
				}
				else
				{
                    PreDead();
                }
			}
		}


		public void PreDead()
		{
			GameStateController.instance.SetAdPanel(true);
			StartCoroutine("OnDeadCoroutine");
        }

		public void Revive()
		{
            AdManager.ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {
                    GameStateController.instance.SetAdPanel(false);
                    StopCoroutine("OnDeadCoroutine");
                    _currentScale -= _scaleMultiplier;
                    StartCoroutine("Invincible");

                    AdManager.clickid = "";
                    AdManager.getClickid();
                    AdManager.apiSend("game_addiction", AdManager.clickid);
                    AdManager.apiSend("lt_roi", AdManager.clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        }

		public IEnumerator Invincible()
		{
			GameStateController.instance.isInvincible = true;
			GetComponent<SpriteRenderer>().color = Color.blue;
			yield return new WaitForSeconds(3);
            GameStateController.instance.isInvincible = false;
            GetComponent<SpriteRenderer>().color = Color.white;
        }

		public IEnumerator OnDeadCoroutine()
		{
			yield return new WaitForSeconds (0.2f);
            _isDead = true;
            GameStateController.instance.OnGameOver();
            Destroy(gameObject);
        }

		private void OnDead()
		{
			_isDead = true;

			GameStateController.instance.OnGameOver ();
			Destroy (gameObject);
		}
	}
}
