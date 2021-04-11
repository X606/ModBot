using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace InternalModBot.Proxies
{
    [Proxy(typeof(Slider))]
    class UnityEngine_UI_SliderProxy : UnityEngine_UI_SelectableProxy
    {
        [MoonSharpHidden]
        Slider _target;

        [MoonSharpHidden]
        public UnityEngine_UI_SliderProxy(Slider target) : base(target)
        {
            _target = target;
        }

        // TODO: Implement this
    }

    [Proxy(typeof(Selectable))]
    class UnityEngine_UI_SelectableProxy : UnityEngine_EventSystems_UIBehaviourProxy
    {
        [MoonSharpHidden]
        Selectable _target;

        [MoonSharpHidden]
        public UnityEngine_UI_SelectableProxy(Selectable target) : base(target)
        {
            _target = target;
        }

        // TODO: Implement this
    }
}
