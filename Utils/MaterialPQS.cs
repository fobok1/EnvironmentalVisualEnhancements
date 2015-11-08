﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Utils
{
    public class MaterialPQS : PQSMod
    {
        Material material;
        Material PQSMaterial;
        PQS parent;
        String materialName = Guid.NewGuid().ToString();

        public void Apply(CelestialBody cb, Material mat)
        {
            KSPLog.print("Applying PQS Material Manager!");
            material = new Material( mat);
            material.name = materialName;

            PQS pqs = null;
            if (cb != null && cb.pqsController != null)
            {
                pqs = cb.pqsController;
                parent = pqs;
                PQSMaterial = pqs.surfaceMaterial;
            }
            else
            {
                KSPLog.print("No PQS!");
            }
            if (pqs != null)
            {
                this.sphere = pqs;
                this.transform.parent = pqs.transform;
                this.transform.localPosition = Vector3.zero;
                this.transform.localRotation = Quaternion.identity;
                this.transform.localScale = Vector3.one;
            }
        }

        public void Remove()
        {
            KSPLog.print("Removing PQS Material Manager!");
            foreach (PQ pq in parent.quads)
            {
                Remove(pq);
            }
            this.sphere = null;
            this.enabled = false;
            this.transform.parent = null;
        }

        private void Remove(PQ quad)
        {
            if (quad != null)
            {
                if (parent.useSharedMaterial)
                {
                    List<Material> materials = new List<Material>(quad.meshRenderer.sharedMaterials);
                    materials.Remove(materials.Find(mat => mat.name.Contains(materialName)));
                    quad.meshRenderer.sharedMaterials = materials.ToArray();
                }
                else
                {
                    List<Material> materials = new List<Material>(quad.meshRenderer.materials);
                    materials.Remove(materials.Find(mat => mat.name.Contains(materialName)));
                    quad.meshRenderer.materials = materials.ToArray();
                }
                if (quad.subNodes != null)
                {
                    foreach (PQ pq in quad.subNodes)
                    {
                        Remove(pq);
                    }
                }
            }
        }

        public override void OnQuadCreate(PQ quad)
        {
            if (parent.useSharedMaterial)
            {
                List<Material> materials = new List<Material>(quad.meshRenderer.sharedMaterials);
                if(!materials.Exists(mat => mat.name.Contains(materialName)))
                {
                    materials.Add(material);
                    quad.meshRenderer.sharedMaterials = materials.ToArray();
                }
            }
            else
            {
                List<Material> materials = new List<Material>(quad.meshRenderer.materials);
                if (!materials.Exists(mat => mat.name.Contains(materialName)))
                {
                    materials.Add(material);
                    quad.meshRenderer.materials = materials.ToArray();
                }
            }
        }

    }
}