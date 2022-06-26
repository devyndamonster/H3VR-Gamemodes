using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TicketDisplay : MonoBehaviour {

	public Slider slider;
	public Text leftText;
	public Text rightText;
	public Image leftColor;
	public Image rightColor;
	
	public void SetTicketValues(int leftTickets, int rightTickets)
    {
		if(leftTickets <= 0)
        {
			slider.value = 0;
        }

		else if(rightTickets <= 0)
        {
			slider.value = 1;
		}

        else
        {
			slider.value = ((float)leftTickets) / (leftTickets + rightTickets);
		}

		leftText.text = Mathf.Max(0, leftTickets).ToString();
		rightText.text = Mathf.Max(0, rightTickets).ToString();
	}

	public void SetTeamColors(Color colorLeft, Color colorRight)
    {
		leftColor.color = colorLeft;
		rightColor.color = colorRight;
    }

}
