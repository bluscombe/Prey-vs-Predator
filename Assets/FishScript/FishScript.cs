/*
    The following license supersedes all notices in the source code.
*/

/*
    Copyright (c) 2018 Kurt Dekker/PLBM Games All rights reserved.
    
    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are
    met:
    
    Redistributions of source code must retain the above copyright notice,
    this list of conditions and the following disclaimer.
    
    Redistributions in binary form must reproduce the above copyright
    notice, this list of conditions and the following disclaimer in the
    documentation and/or other materials provided with the distribution.
    
    Neither the name of the Kurt Dekker/PLBM Games nor the names of its
    contributors may be used to endorse or promote products derived from
    this software without specific prior written permission.
    
    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
    IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
    TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
    PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
    HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
    TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishScript : MonoBehaviour
{
	float LowFrequencyTheta;
	float HighFrequencyTheta;

	float WriggleOffsetAngle;

	float Speed;

	float myAge;

	void Start()
	{
		myAge = Random.Range (0.0f, Mathf.PI * 2);

		LowFrequencyTheta = Random.Range (1.6f, 3.0f);

		HighFrequencyTheta = Random.Range (3.0f, 4.0f);

		WriggleOffsetAngle = Random.Range (30.0f, 40.0f);

		Speed = Random.Range (2.0f, 3.0f);
	}

	void Update ()
	{
		myAge += Time.deltaTime;

		// this is the rate at which the fish wriggles, then doesn't wriggle
		float myActivityLevel = Mathf.Sin (myAge * LowFrequencyTheta);
		if (myActivityLevel < 0.1f)
		{
			myActivityLevel = 0.1f;
		}

		// this is the wriggle
		HighFrequencyTheta += myActivityLevel;

		float fishAngle = Mathf.Sin (HighFrequencyTheta) * WriggleOffsetAngle * myActivityLevel;

		transform.rotation = Quaternion.Euler (0, fishAngle, 0);

		transform.position += Vector3.right * myActivityLevel * Speed * Time.deltaTime;
	}
}
