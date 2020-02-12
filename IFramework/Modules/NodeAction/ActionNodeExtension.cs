﻿using IFramework.Modules.Coroutine;
using System;
using System.Collections;

namespace IFramework.Modules.NodeAction
{
    [FrameworkVersion(8)]
    public static class ActionNodeExtension
    {
        public static IEnumerator IActionEnumerator<T>(this T self) where T : ActionNode
        {
            while (self.MoveNext())
            {
                yield return null;
            }
        }
        public static T Run<T>(this T self, CoroutineModule moudle) where T : ActionNode
        {
            moudle.StartCoroutine(self.IActionEnumerator());
            return self;
        }
        public static T Run<T>(this T self) where T : ActionNode
        {
            self.env.modules.Coroutine.StartCoroutine(self.IActionEnumerator());
            return self;
        }

        private static T Allocate<T>(FrameworkEnvironment env) where T : ActionNode
        {
            T t = RecyclableObject.Allocate<T>(env);
            while (t.disposed)
                t = RecyclableObject.Allocate<T>(env);
            return t;
        }
        private static T Allocate<T>(int envIndex) where T : ActionNode
        {
            T t = RecyclableObject.Allocate<T>(envIndex);
            while (t.disposed)
                t = RecyclableObject.Allocate<T>(envIndex);
            return t;
        }

        public static SequenceNode Sequence(this object self, int envIndex,bool autoRecyle = true)
        {
            SequenceNode node = Allocate<SequenceNode>(envIndex);
            node.Config(autoRecyle);
            return node;
        }
        public static SequenceNode Sequence(this object self, FrameworkEnvironment env, bool autoRecyle = true)
        {
            SequenceNode node = Allocate<SequenceNode>(env);
            node.Config(autoRecyle);
            return node;
        }

        public static T OnCompelete<T>(this T self, Action<T> action) where T : ActionNode
        {
            self.OnCompelete(() => { action(self); });
            return self;
        }
        public static T OnBegin<T>(this T self, Action<T> action) where T : ActionNode
        {
            return self.OnBegin(() => { action(self); });
        }
        public static T OnDispose<T>(this T self, Action<T> action) where T : ActionNode
        {
            return self.OnDispose(() => { action(self); });
        }
        public static T OnRecyle<T>(this T self, Action<T> action) where T : ActionNode
        {
            return self.OnRecyle(() => { action(self); });
        }
        public static T OnCompelete<T>(this T self, Action action) where T : ActionNode
        {
            self.onCompelete += action;
            return self;
        }
        public static T OnBegin<T>(this T self, Action action) where T : ActionNode
        {
            self.onBegin += action;
            return self;
        }
        public static T OnDispose<T>(this T self, Action action) where T : ActionNode
        {
            self.onDispose += action;
            return self;
        }
        public static T OnRecyle<T>(this T self, Action action) where T : ActionNode
        {
            self.onRecyle += action;
            return self;
        }

        public static T TimeSpan<T>(this T self, TimeSpan timeSpan, bool autoRecyle = false) where T : ContainerNode
        {
            TimeSpanNode node = Allocate<TimeSpanNode>(self.env);
            node.Config(timeSpan, autoRecyle);
            self.Append(node);
            return self;
        }
        public static T Until<T>(this T self, Func<bool> func, bool autoRecyle = false) where T : ContainerNode
        {
            UntilNode node = Allocate<UntilNode>(self.env);
            node.Config(func, autoRecyle);
            self.Append(node);
            return self;
        }
        public static T Event<T>(this T self, Action action, bool autoRecyle = false) where T : ContainerNode
        {
            EventNode node = Allocate<EventNode>(self.env);
            node.Config(action, autoRecyle);
            self.Append(node);
            return self;
        }
        public static T Repeat<T>(this T self, Action<RepeatNode> action, int repeat = 1, bool autoRecyle = false) where T : ContainerNode
        {
            RepeatNode node = Allocate<RepeatNode>(self.env);
            node.Config(repeat, autoRecyle);
            if (action != null)
                action(node);
            self.Append(node);
            return self;
        }

        public static T Sequence<T>(this T self, Action<SequenceNode> action, bool autoRecyle = false) where T : ContainerNode
        {
            SequenceNode node = Allocate<SequenceNode>(self.env);
            node.Config(autoRecyle);
            if (action != null)
                action(node);
            self.Append(node);
            return self;
        }
        public static T Spawn<T>(this T self, Action<SpawnNode> action, bool autoRecyle = false) where T : ContainerNode
        {
            SpawnNode node = Allocate<SpawnNode>(self.env);
            node.Config(autoRecyle);
            if (action != null)
                action(node);
            self.Append(node);
            return self;
        }

      
    }

}