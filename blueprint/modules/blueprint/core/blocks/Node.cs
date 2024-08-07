﻿using blueprint.modules.blueprint.core.component;
using blueprint.modules.blueprint.core.fields;
using blueprint.modules.node.types;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Net;

namespace blueprint.modules.blueprint.core.blocks
{
    public class Node : Block
    {
        public Node from { get; set; }
        public Script script { get; set; }
        // public Dictionary<string, Field> fields { get; set; }
        public Field fields { get; set; }
        public List<component.ComponentBase> components { get; set; }
        public Dictionary<string, object> data { get; set; }
        public Node() : base()
        {
            // fields = new Dictionary<string, Field>();
            fields = new Field();
            components = new List<component.ComponentBase>();
            data = new Dictionary<string, object>();
            coordinate = new Coordinate() { h = 10, w = 10 };
        }
        public object GetField(string address)
        {
            return fields.Value(address, this);
        }
        public void SetField(string address, object value)
        {
            fields.SetValue(address, value);
        }
        public void FieldPush(string address, object value)
        {
            fields.PushValue(address, value);
        }
        public void CallStart()
        {
            CallStart(null);
        }
        public void CallStart(Node fromNode)
        {
            from = fromNode;
            script?.Invoke("node", new runtime.Node(this), "start");
        }
        public void InvokeFunction(string function)
        {
            script?.Invoke("node", new runtime.Node(this), function);
        }
        public void ExecuteNode(string address)
        {
            var field = GetField(address);
            if (field != null && field is List<Field> fieldArray)
            {
                foreach (var item in fieldArray)
                {
                    var nodeId = item.AsString(this);
                    var node = this.bind_blueprint.nodes.FirstOrDefault(i => i.id == nodeId);
                    if (node != null)
                        node.CallStart(this);
                }
            }
        }
        public void ExecuteNode(string address, int position)
        {
            var field = GetField(address);
            if (field != null && field is List<Field> fieldArray)
            {
                var nodeId = fieldArray[position].AsString(this);

                var node = this.bind_blueprint.nodes.FirstOrDefault(i => i.id == nodeId);
                if (node != null)
                    node.CallStart(this);
            }
        }
        public int GetFieldArraySize(string address)
        {
            return fields.GetArraySize(address);
        }
        public void set_output(object value)
        {
            set_data("_$$_OUTPUT_$$_", value);
        }
        public object get_output()
        {
            return get_data("_$$_OUTPUT_$$_", null);
        }
        public object get_data(string name, object alter)
        {
            if (data.TryGetValue(name, out var _val))
                return _val;
            else
                return alter;
        }
        public void set_data(string name, object value)
        {
            if (!data.ContainsKey(name))
                data.Add(name, value);
            else
                data[name] = value;
        }
        public void BindNode(Node node)
        {
            BindNode("next", node);
        }
        public void BindNode(string address, Node node)
        {
            var field = fields.GetField(address);
            if (field.AsArrayList == null)
                field.AsArrayList = new List<Field>();
            field.AsArrayList.Add(new Field() { value = node });
        }
        public void UnBindNode(Node node)
        {
            UnBindNode("next", node);
        }
        public void UnBindNode(string address, Node node)
        {
            var field = fields.GetField(address);
            field.AsArrayList.RemoveAll(i => i.value == node);
        }
        public T AddComponent<T>() where T : ComponentBase
        {
            return AddComponent<T>(null);
        }
        public T AddComponent<T>(string name) where T : ComponentBase
        {
            var instance = (ComponentBase)Activator.CreateInstance<T>();
            instance.name = name;
            instance.node = this;
            components.Add(instance);
            return GetComponent<T>();
        }
        public T GetComponent<T>() where T : ComponentBase
        {
            foreach (var c in components)
            {
                if (c is T t)
                    return t;
            }
            return default;
        }
        public List<T> GetComponents<T>() where T : ComponentBase
        {
            List<T> result = new List<T>();
            foreach (var c in components)
            {
                if (c is T t)
                    result.Add(t);
            }
            return result;
        }
        public bool HasComponent<T>() where T : ComponentBase
        {
            foreach (var c in components)
            {
                if (c is T t)
                    return true;
            }
            return false;
        }
    }
}
