﻿using System.Collections.Generic;
using DotNetCore.CAP.NodeDiscovery;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCore.CAP.Dashboard.Pages
{
    internal partial class NodePage
    {
        private INodeDiscoveryProvider _discoveryProvider;
        private IList<Node> _nodes;

        public NodePage()
        {
        }

        public NodePage(string id)
        {
            CurrentNodeId = id;
        }

        public string CurrentNodeId { get; set; }

        public IList<Node> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _discoveryProvider = RequestServices.GetService<INodeDiscoveryProvider>();
                    _nodes = _discoveryProvider.GetNodes().GetAwaiter().GetResult();
                }
                return _nodes;
            }
        }
    }
}