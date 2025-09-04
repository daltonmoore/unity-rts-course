using Units;
using UnityEngine;

namespace Commands
{
    public interface ICommand
    {
        bool CanHandle(AbstractCommandable commandable, RaycastHit hit);
        void Handle(AbstractCommandable commandable, RaycastHit hit);
    }
}