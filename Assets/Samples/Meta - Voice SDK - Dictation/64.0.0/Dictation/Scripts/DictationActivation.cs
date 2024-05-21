/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using Meta.WitAi.Dictation;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meta.Voice.Samples.Dictation
{
    public class DictationActivation : MonoBehaviour
    {
        [FormerlySerializedAs("dictation")]
        [SerializeField] private DictationService _dictation;

        private void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                _dictation.ActivateImmediately();
            }
        }

        public void ToggleActivation()
        {
            if (_dictation.MicActive)
            {
                _dictation.Deactivate();
            }
            else
            {
                _dictation.Activate();
            }
        }
    }
}
