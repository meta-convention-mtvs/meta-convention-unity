// Copyright (C) 2023 ricimi. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at https://unity.com/legal/as-terms.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ricimi
{
	// This UI component is a specialized selection slider that allows you to
	// scroll between different text-based options.
	public class Customization_GenderSelection : MonoBehaviour
	{
		public List<string> Options;
		public Text OptionText;

		private int selectedOption;

		private void Start()
		{
			ChangeSelection();
		}

		public void OnPrevButtonClicked()
		{
			selectedOption--;
			if (selectedOption < 0)
			{
				selectedOption = Options.Count - 1; 
			}

			ChangeSelection();
		}

		public void OnNextButtonClicked()
		{
			selectedOption = (selectedOption + 1) % Options.Count;

			ChangeSelection();
		}

		private void ChangeSelection()
		{
			OptionText.text = Options[selectedOption];
		}
	}
}