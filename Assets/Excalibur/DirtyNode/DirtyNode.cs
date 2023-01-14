using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Excalibur;
using UnityEngine.UI;
using TMPro;

namespace Excalibur
{
	public class DirtyNode : MonoBehaviour, IDirty
	{
		protected bool m_Dirty;

		public void SetDirty()
		{
			m_Dirty = true;
        }

		protected virtual void Dirty()
		{

		}

		protected virtual void Awake()
		{
			SetDirty();
        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void Start()
		{
			
		}

        protected virtual void Update()
		{
			if (m_Dirty)
			{
				Dirty();
				m_Dirty = false;
			}
		}

        protected virtual void OnDisable()
		{
			
		}

        protected virtual void OnDestroy()
		{
			
		}
	}
}
