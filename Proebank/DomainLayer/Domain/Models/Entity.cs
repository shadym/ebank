﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public interface IEntity
    {
        Guid Id { get; }
        bool IsRemoved { get; }
    }

    public abstract class Entity : IEntity
    {
        public Guid Id { get; set; }

        public bool IsRemoved { get; set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
            IsRemoved = false;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Entity;
            return other != null
                && other.Id.Equals(Id)
                && other.GetType().FullName == GetType().FullName;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
