// GENERATED AUTOMATICALLY FROM 'Assets/Input/MouseHelperStuff.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @MouseHelperStuff : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @MouseHelperStuff()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""MouseHelperStuff"",
    ""maps"": [
        {
            ""name"": ""MouseHelper"",
            ""id"": ""e7584a8c-9abc-43f1-b4d3-8cafd8196ab2"",
            ""actions"": [
                {
                    ""name"": ""Scroll"",
                    ""type"": ""PassThrough"",
                    ""id"": ""f5cf5cc6-7f72-4304-99a4-1ec04f57d700"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ea894dfd-c7ad-481d-b3df-5a0aa2cdc69d"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // MouseHelper
        m_MouseHelper = asset.FindActionMap("MouseHelper", throwIfNotFound: true);
        m_MouseHelper_Scroll = m_MouseHelper.FindAction("Scroll", throwIfNotFound: true);
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

    // MouseHelper
    private readonly InputActionMap m_MouseHelper;
    private IMouseHelperActions m_MouseHelperActionsCallbackInterface;
    private readonly InputAction m_MouseHelper_Scroll;
    public struct MouseHelperActions
    {
        private @MouseHelperStuff m_Wrapper;
        public MouseHelperActions(@MouseHelperStuff wrapper) { m_Wrapper = wrapper; }
        public InputAction @Scroll => m_Wrapper.m_MouseHelper_Scroll;
        public InputActionMap Get() { return m_Wrapper.m_MouseHelper; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseHelperActions set) { return set.Get(); }
        public void SetCallbacks(IMouseHelperActions instance)
        {
            if (m_Wrapper.m_MouseHelperActionsCallbackInterface != null)
            {
                @Scroll.started -= m_Wrapper.m_MouseHelperActionsCallbackInterface.OnScroll;
                @Scroll.performed -= m_Wrapper.m_MouseHelperActionsCallbackInterface.OnScroll;
                @Scroll.canceled -= m_Wrapper.m_MouseHelperActionsCallbackInterface.OnScroll;
            }
            m_Wrapper.m_MouseHelperActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Scroll.started += instance.OnScroll;
                @Scroll.performed += instance.OnScroll;
                @Scroll.canceled += instance.OnScroll;
            }
        }
    }
    public MouseHelperActions @MouseHelper => new MouseHelperActions(this);
    public interface IMouseHelperActions
    {
        void OnScroll(InputAction.CallbackContext context);
    }
}
