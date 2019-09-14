﻿using System;
using System.Collections.Generic;
using System.Text;
using static Dqlite.Net.NativeMethods;

namespace Dqlite.Net
{
    public class DqliteNode : IDisposable
    {

        static DqliteNode()
        {
            var rc = sqlite3_config(1);
        }

        public ulong Id { get; }
        public string Address { get; }
        
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
            CheckError(dqlite_node_start(this.node));
            this.active = true;
        }

        public void Stop()
        {
            CheckError(dqlite_node_stop(this.node));
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

        public static DqliteNode Create(ulong id, string address, string dataDir)
        {
            CheckError(dqlite_node_create(id, address, dataDir, out var node));
            CheckError(dqlite_node_set_bind_address(node, address));
            CheckError(dqlite_node_set_network_latency(node, 20 * 1000 * 1000000UL));
            
            return new DqliteNode(node, id, address);
        }
    }
}
