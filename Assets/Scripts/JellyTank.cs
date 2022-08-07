using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lib;
using UnityEngine;

public class JellyTank : MonoBehaviour
{
    private void Start()
    {
        var trackNames = new[] {"Edge_L", "Edge_R", "Middle_", "Middle_L", "Middle_R", "Side_L", "Side_R"};

        var track = GetComponentsInChildren<Transform>().Where(_transform =>
            trackNames.Any(trackName => _transform.gameObject.name.Contains(trackName)));

        GameObject[] FindTracks(string key) => track
            .Where(_transform => _transform.gameObject.name.Contains(key))
            .Select(_transform => _transform.gameObject)
            .Where(_gameObject => int.TryParse(_gameObject.name.Replace(key, "").Replace("_", ""), out _))
            .OrderBy(_gameObject => int.Parse(_gameObject.name.Replace(key, "").Replace("_", "")))
            .ToArray();

        // var Edge_L_ = FindTracks("Edge_L_");
        // var Edge_R = FindTracks("Edge_R");
        // var Middle_ = FindTracks("Middle_");
        // var Middle_L = FindTracks("Middle_L");
        // var Middle_R = FindTracks("Middle_R");
        // var Side_L = FindTracks("Side_L");
        // var Side_R = FindTracks("Side_R");

        void SetupConnections(GameObject[] tracks) => tracks.ForEachIndexed((track, i) =>
        {
            var springs = track.GetComponents<SpringJoint>();
            int leftIndex = i - 1 < 0 ? tracks.Length - 1 : i - 1;
            int rightIndex = i + 1 >= tracks.Length ? 0 : i + 1;
            springs[0].connectedBody = tracks[leftIndex].GetComponent<Rigidbody>();
            springs[1].connectedBody = tracks[rightIndex].GetComponent<Rigidbody>();
        });

        trackNames
            .Select(FindTracks)
            .ForEach(SetupConnections);
        // SetupConnections(Edge_L_);
        // SetupConnections(Edge_R);
        // SetupConnections(Middle_);
        // SetupConnections(Middle_L);
        // SetupConnections(Middle_R);
        // SetupConnections(Side_L);
        // SetupConnections(Side_R);

        GetComponentsInChildren<ConfigurableJoint>().ForEach(joint =>
            joint.axis = joint.transform.InverseTransformDirection(joint.transform.localPosition.Copy(x: 0f)));
    }
}