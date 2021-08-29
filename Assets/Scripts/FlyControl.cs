using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyControl : MonoBehaviour
{



    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
		{

			GameObject flySprite = transform.GetChild(0).gameObject;
			SpriteRenderer renderer = flySprite.GetComponent<SpriteRenderer>();
			renderer.color = new Color(255, 255, 255, 0);
			BoxCollider2D boxCol = transform.GetComponent<BoxCollider2D>();
			Destroy(boxCol);

			StartCoroutine(WaitForRespawn(renderer));
			//Destroy(transform.gameObject);
			//Destroy(flySprite);

		}
	}

	private IEnumerator WaitForRespawn(SpriteRenderer renderer)
    {
		yield return new WaitForSeconds(3.5f);

		float duration = 1.5f;
		Color start = new Color(255, 255, 255, 0);
		Color end = new Color(255, 255, 255, 255);
		for (float t = 0f; t < duration; t += Time.deltaTime)
		{
			float normalizedTime = t / (duration * 200);
			renderer.color = Color.Lerp(start, end, normalizedTime);
			yield return null;
		}
		renderer.color = end;
		BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
		boxCollider.isTrigger = true;
		boxCollider.size = new Vector2(0.39f, 0.3f);

		yield return null;
    }		
}
