using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Common;
using UnityEngine;

namespace Assets.Manager
{
    public class ResourceMngr:MonoBehaviour
    {
        private void Awake()
        {
            GameApp.Instance.ResourceMngr = this;
        }

        public Sprite LoadSprite(string path)
        {
            return Resources.Load<Sprite>(path);
        }

        public AudioClip LoadAudioClip(string path)
        {
            return Resources.Load<AudioClip>(path);
        }

        public GameObject LoadGameObject(string path)
        {
            GameObject go;
            if (GameData.Instance.GODict.TryGetValue(path, out go))
                return go;
            go = Resources.Load<GameObject>(path);
            GameData.Instance.GODict.Add(path,go);
            return go;
        }

        public GameObject InstantiateGameObject(string path,Transform parent,Vector3 pos)
        {
            GameObject go = LoadGameObject(path);
            GameObject obj = Instantiate(go);
            if(parent != null)
                obj.transform.SetParent(parent);
            obj.transform.localPosition = pos;
            obj.transform.localScale = Vector3.one;;
            obj.SetActive(true);
            return obj;
        }

        public string LoadText(string path)
        {
            TextAsset txt = Resources.Load<TextAsset>(path);
            return txt == null ? string.Empty : txt.text;
        }
    }
}
