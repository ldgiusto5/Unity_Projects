using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
namespace RPG.SceneManagement
{
	public class Portal : MonoBehaviour
	{
		enum DestinationIdentifier
		{
			A, B, C, D, E
		}
		[SerializeField] int sceneToLoad = -1;
		[SerializeField] Transform spawnPoint;
		[SerializeField] DestinationIdentifier destination;
		[SerializeField] float fadeOutTime = 0.5f;
		[SerializeField] float fadeInTime = 0.5f;
		[SerializeField] float fadeWaitTime = 0.5f;

		private void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player")
			{
				StartCoroutine(Transition());
			}
		}
		private IEnumerator Transition()
		{
			if (sceneToLoad < 0)
			{
				Debug.LogError("Scene to load error in portal");
				yield break;
			}

			DontDestroyOnLoad(gameObject);

			Fader fader = FindAnyObjectByType<Fader>();

			yield return fader.FadeOut(fadeOutTime);
			yield return SceneManager.LoadSceneAsync(sceneToLoad);
			print("Scene Loaded");

			Portal otherPortal = GetOtherPortal();
			UpdatePlayer(otherPortal);
			yield return new WaitForSeconds(fadeWaitTime);
			yield return fader.FadeIn(fadeInTime);
			Destroy(gameObject);
		}

		private void UpdatePlayer(Portal otherPortal)
		{
			GameObject player = GameObject.FindWithTag("Player");
			player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
			player.transform.rotation = otherPortal.spawnPoint.rotation;
		}

		private Portal GetOtherPortal()
		{
			foreach (Portal portal in FindObjectsOfType<Portal>())
			{
				if (portal == this) continue;
				if (portal.destination != destination) continue;

				return portal;
			}

			return null;
		}
	}
}