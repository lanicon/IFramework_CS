﻿using System;
using System.Reflection;

namespace IFramework.Moudles
{
    public abstract class FrameworkMoudle : IFrameworkMoudle
    {
        public static FrameworkMoudle CreatInstance(FrameworkMoudleContainer container, Type type, string chunck = "Framework")
        {
            FrameworkMoudle moudle = Activator.CreateInstance(type,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, null, null) as FrameworkMoudle;

            if (moudle != null)
            {
                moudle._binded = false;
                moudle._disposed = false;
                moudle._chunck = chunck;
                moudle._moudleType = moudle.GetType().Name;
                moudle._name = string.Format("{0}.{1}", moudle._chunck, moudle._moudleType);

                if (container != null)
                    moudle.Bind(container);
                moudle.Awake();
                moudle.enable = true;
            }
            else
                Log.E(string.Format("Type: {0} Non Public Ctor With 0 para Not Find", type));

            return moudle;
        }
        public static FrameworkMoudle CreatInstance(FrameworkMoudleContainer container, Type type)
        {
            return CreatInstance(container, type);
        }
        public static FrameworkMoudle CreatInstance(Type type, string chunck = "Framework")
        {
            return CreatInstance(null, type, chunck);
        }
        public static T CreatInstance<T>(FrameworkMoudleContainer container) where T : FrameworkMoudle
        {
            return CreatInstance(container, typeof(T)) as T;
        }
        public static T CreatInstance<T>(FrameworkMoudleContainer container, string chunck = "Framework") where T : FrameworkMoudle
        {
            return CreatInstance(container, typeof(T), chunck) as T;
        }
        public static T CreatInstance<T>(string chunck = "Framework") where T : FrameworkMoudle
        {
            return CreatInstance(typeof(T), chunck) as T;
        }


        public void Bind(FrameworkMoudleContainer container)
        {
            if (this._container!=null)
            {
                Log.E(string.Format("Have Bind One Container chunck: {0},You Can UnBind First", this._container.chunck));
                return;
            }
            this._binded = true;
            this._chunck = container.chunck;
            this._name = string.Format("{0}.{1}", this._chunck, this._moudleType);
            this._container = container;

            container.AddMoudle(this);
        }
        public void UnBind(bool dispose)
        {
            if (this._container != null)
                this._container.RemoveBindMoudle(this);
            this._binded = false;
            this._container = null;
            if (dispose)
                Dispose();
        }

        private FrameworkMoudleContainer _container;
        private string _name;
        private bool _disposed;
        private bool _enable;
        private string _chunck;
        private string _moudleType;
        private bool _binded;

        public string moudeType { get { return _moudleType; } }
        public string chunck { get { return _chunck; } }
        public string name { get { return _name; } }
        public bool binded { get { return _binded; } }
        public bool disposed { get { return _disposed; } }
        public bool enable
        {
            get { return _enable; }
            set
            {
                if (_enable != value)
                    _enable = value;
                if (_enable)
                    OnEnable();
                else
                    OnDisable();
            }
        }
        public FrameworkMoudleContainer container { get { return _container; } }

        public void SetActive(bool enable) { this.enable = enable; }

        protected FrameworkMoudle() { }

        public void Dispose()
        {
            enable = false;
            _disposed = true;
            _name = string.Empty;
            OnDispose();
        }
        public void Update()
        {
            if (!enable || disposed) return;
            OnUpdate();
        }



        protected abstract void Awake();
        protected abstract void OnDispose();
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected abstract void OnUpdate();

    }

}