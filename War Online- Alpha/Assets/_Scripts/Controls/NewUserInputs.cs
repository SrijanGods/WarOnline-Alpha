// GENERATED AUTOMATICALLY FROM 'Assets/_Settings/Game Inputs.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace _Scripts.Controls
{
    public class @UserInputs : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @UserInputs()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Game Inputs"",
    ""maps"": [
        {
            ""name"": ""Tank"",
            ""id"": ""dd84e022-e98a-466e-95fa-26a4431b82fb"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""db49aff5-f183-49b4-a3c9-e249991f7696"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""PassThrough"",
                    ""id"": ""57eb42f1-4bba-4272-be3f-68b5a00ecaca"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Recenter"",
                    ""type"": ""PassThrough"",
                    ""id"": ""9d10cddb-b1f4-4989-99b3-fc1f9f5a2149"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""PassThrough"",
                    ""id"": ""5d75f3a0-834d-4831-9d7b-0bf4b1149ba2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""1fc5f44f-4476-4b95-84cc-40ead1753249"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5eee87d4-4fcb-400b-9806-814169350039"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f545574a-1bfa-4026-8871-9c8ac791a3a1"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""2f559da4-240f-4860-ba0a-0933880ba811"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""70d4e218-85b6-4629-8236-46c1b0b4f62d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""d9f93f83-dac5-45b6-9761-cc82d21eba66"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""829aa6f4-d779-4a18-8f85-3d4034476f0f"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2e8266d6-7b2e-473d-a744-835ea8ad4ecc"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Recenter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Tank
            m_Tank = asset.FindActionMap("Tank", throwIfNotFound: true);
            m_Tank_Move = m_Tank.FindAction("Move", throwIfNotFound: true);
            m_Tank_Look = m_Tank.FindAction("Look", throwIfNotFound: true);
            m_Tank_Recenter = m_Tank.FindAction("Recenter", throwIfNotFound: true);
            m_Tank_Fire = m_Tank.FindAction("Fire", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Tank
        private readonly InputActionMap m_Tank;
        private ITankActions m_TankActionsCallbackInterface;
        private readonly InputAction m_Tank_Move;
        private readonly InputAction m_Tank_Look;
        private readonly InputAction m_Tank_Recenter;
        private readonly InputAction m_Tank_Fire;
        public struct TankActions
        {
            private @UserInputs m_Wrapper;
            public TankActions(@UserInputs wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Tank_Move;
            public InputAction @Look => m_Wrapper.m_Tank_Look;
            public InputAction @Recenter => m_Wrapper.m_Tank_Recenter;
            public InputAction @Fire => m_Wrapper.m_Tank_Fire;
            public InputActionMap Get() { return m_Wrapper.m_Tank; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(TankActions set) { return set.Get(); }
            public void SetCallbacks(ITankActions instance)
            {
                if (m_Wrapper.m_TankActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_TankActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_TankActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_TankActionsCallbackInterface.OnMove;
                    @Look.started -= m_Wrapper.m_TankActionsCallbackInterface.OnLook;
                    @Look.performed -= m_Wrapper.m_TankActionsCallbackInterface.OnLook;
                    @Look.canceled -= m_Wrapper.m_TankActionsCallbackInterface.OnLook;
                    @Recenter.started -= m_Wrapper.m_TankActionsCallbackInterface.OnRecenter;
                    @Recenter.performed -= m_Wrapper.m_TankActionsCallbackInterface.OnRecenter;
                    @Recenter.canceled -= m_Wrapper.m_TankActionsCallbackInterface.OnRecenter;
                    @Fire.started -= m_Wrapper.m_TankActionsCallbackInterface.OnFire;
                    @Fire.performed -= m_Wrapper.m_TankActionsCallbackInterface.OnFire;
                    @Fire.canceled -= m_Wrapper.m_TankActionsCallbackInterface.OnFire;
                }
                m_Wrapper.m_TankActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Look.started += instance.OnLook;
                    @Look.performed += instance.OnLook;
                    @Look.canceled += instance.OnLook;
                    @Recenter.started += instance.OnRecenter;
                    @Recenter.performed += instance.OnRecenter;
                    @Recenter.canceled += instance.OnRecenter;
                    @Fire.started += instance.OnFire;
                    @Fire.performed += instance.OnFire;
                    @Fire.canceled += instance.OnFire;
                }
            }
        }
        public TankActions @Tank => new TankActions(this);
        public interface ITankActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnLook(InputAction.CallbackContext context);
            void OnRecenter(InputAction.CallbackContext context);
            void OnFire(InputAction.CallbackContext context);
        }
    }
}
