﻿using System;
using System.IO;
using static Dqlite.Net.NativeMethods;

namespace Dqlite.Net
{
    public class DqliteNode : IDisposable
    {
 
        static DqliteNode()
        {

        }

        public ulong Id { get; }
        public string Address { get; }
        public string BindAddress => dqlite_node_get_bind_address(this.node);
        
        private bool active;
        private readonly IntPtr node;

        private DqliteNode(IntPtr node, ulong id, string address)
        {
            this.Id = id;
            this.node = node;
            this.Address = address;
        }

        public void Start()
        {
            CheckError(dqlite_node_start(this.node), this.node);
            this.active = true;
        }

        public void Stop()
        {
            CheckError(dqlite_node_stop(this.node), this.node);
            this.active = false;
        }

        public void Dispose()
        {
            if (this.active)
            {
                this.Stop();
            }

            dqlite_node_destroy(this.node);
        }

        public static ulong GenerateNodeId(string address)
            => dqlite_generate_node_id(address);
            
        public static DqliteNode Create(string address, string dataDir, DqliteNodeOptions options = null)
        {
            var id = dqlite_generate_node_id(address);
            return Create(id, address, dataDir, options);
        }

        public static DqliteNode Create(ulong id, string address, string dataDir, DqliteNodeOptions options = null)
        {
            Directory.CreateDirectory(dataDir);
            CheckError(dqlite_node_create(id, address, dataDir, out var node), node);
            
            if(options?.DialFunction != null)
            {
                CheckError(dqlite_node_set_connect_func(node, options.DialFunction, IntPtr.Zero), node);
            }

            CheckError(dqlite_node_set_bind_address(node, options?.Address ?? address), node);
            
            if(options != null && options.NetworkLatency != default(TimeSpan))
            {
                CheckError(dqlite_node_set_network_latency(node, (ulong)options.NetworkLatency.TotalMilliseconds *  1000000UL), node);
            }
            
            return new DqliteNode(node, id, address);
        } 
    }
}
