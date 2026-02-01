using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
	public class TitleAnimation  : MonoBehaviour
	{
		[SerializeField] TMPro.TextMeshProUGUI _wText;
		[SerializeField] TMPro.TextMeshProUGUI _allhackText;

		private Vector2 _wStartPos;
		private Vector2 _allhackPos;
		
		private void Start()
		{
			// store original transforms
			_wStartPos = _wText.transform.GetComponent<RectTransform>().anchoredPosition;
			_allhackPos = _allhackText.transform.GetComponent<RectTransform>().anchoredPosition;
		}

		public void Run()
		{
			StartCoroutine(AnimationCoroutine());
		}

		public void Reset()
		{
			StopAllCoroutines();
			_wText.text = "W";
			_wText.transform.GetComponent<RectTransform>().anchoredPosition = _wStartPos;
		}

		IEnumerator AnimationCoroutine()
		{
			_wText.text = "<s>W</s>";
			
			_wText.fontMaterial.EnableKeyword("UNDERLAY_ON");
			_allhackText.fontMaterial.EnableKeyword("UNDERLAY_ON");
			yield return new WaitForSeconds(0.05f);
			_wText.fontMaterial.DisableKeyword("UNDERLAY_ON");
			_allhackText.fontMaterial.DisableKeyword("UNDERLAY_ON");
			yield return new WaitForSeconds(0.05f);
			_wText.fontMaterial.EnableKeyword("UNDERLAY_ON");
			_allhackText.fontMaterial.EnableKeyword("UNDERLAY_ON");
			yield return new WaitForSeconds(0.05f);
			_wText.fontMaterial.DisableKeyword("UNDERLAY_ON");
			_allhackText.fontMaterial.DisableKeyword("UNDERLAY_ON");
			yield return new WaitForSeconds(0.05f);
			_wText.fontMaterial.EnableKeyword("UNDERLAY_ON");
			_allhackText.fontMaterial.EnableKeyword("UNDERLAY_ON");
			yield return new WaitForSeconds(0.05f);
			_wText.fontMaterial.DisableKeyword("UNDERLAY_ON");
			_allhackText.fontMaterial.DisableKeyword("UNDERLAY_ON");
			yield return new WaitForSeconds(0.5f);




			float t = 0;
			while (t < 3)
			{
				Vector3 target = Vector3.Slerp(_wStartPos, _wStartPos + Vector2.down * 1000, t*1.2f);
				_wText.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(_wStartPos.x, target.y);
				t += Time.deltaTime;
				yield return null;
			}
		}
	}
}