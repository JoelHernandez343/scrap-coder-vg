// Joel Harim Hernández Javier @ 2022
// Github: https://github.com/JoelHernandez343

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapCoder.InputManagment {
    public interface IFocusable {
        void GetRemoverOwnership(GameObject remover);
        void LoseFocus();
        void GetFocus();
        bool HasFocus();
    }
}