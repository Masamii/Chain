using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    [SerializeField] Transform m_ChainStart = default;
    [SerializeField] Transform m_ChainEnd = default;
    [SerializeField] GameObject m_PartPrefab = default;
    [SerializeField, Range(4, 32)] int m_Size = 16;
    [SerializeField, Range(0.2f, 5.0f)] float m_Distance = 0.5f;
    [SerializeField] AnimationCurve m_SmoothingCurve = AnimationCurve.EaseInOut(0.0f, 1.0f, 1.0f, 0.5f);
    [SerializeField, Range(1.0f, 100.0f)] float m_SmoothingMultiplyEtoS = 50.0f;
    [SerializeField, Range(1.0f, 100.0f)] float m_SmoothingMultiplyStoE = 50.0f;
    [SerializeField] float m_GroundHeight = 0.1f;

    List<Transform> m_Parts = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        for (int _idx = 0; _idx < m_Size; _idx++)
        {
            var _go = GameObject.Instantiate(m_PartPrefab, transform);
            _go.name = $"Part {_idx + 1}";
            m_Parts.Add(_go.transform);
        }

        var _forward = m_ChainStart.forward;
        var _prevPos = m_ChainStart.position;
        foreach (var _part in m_Parts)
        {
            _part.position = _prevPos + _forward * m_Distance;
            _part.rotation = Quaternion.LookRotation(_prevPos - _part.position);
            _prevPos = _part.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ‹^Ž—d—Í
        foreach (var _part in m_Parts)
        {
            if (m_GroundHeight < _part.position.y)
            {
                _part.position += Vector3.down * Time.deltaTime;
                if (_part.position.y < m_GroundHeight)
                {
                    _part.position = new Vector3(_part.position.x, m_GroundHeight, _part.position.z);
                }
            }
        }

        var _prev = m_ChainEnd;
        int _progress = 0;
        for (int _idx = m_Parts.Count - 1; 0 <= _idx; _idx--, _progress++)
        {
            var _part = m_Parts[_idx];

            float _smooting = m_SmoothingCurve.Evaluate((float)_progress / m_Parts.Count) * m_SmoothingMultiplyEtoS;

            // ‹K’è‚æ‚è‚à‹——£‚ª—£‚ê‚Ä‚¢‚½‚ç‹ß‚Ã‚¯‚é
            var _diff = _prev.position - _part.position;
            if (m_Distance < _diff.magnitude)
            {
                var _destination = _prev.position + (_part.position - _prev.position).normalized * m_Distance;
                var _smoothDest = Vector3.Lerp(_part.position, _destination, Time.deltaTime * _smooting);
                _part.position = _smoothDest;
            }
            _prev = _part;
        }

        _prev = m_ChainStart;
        _progress = 0;
        for (int _idx = 0; _idx < m_Parts.Count; _idx++)
        {
            var _part = m_Parts[_idx];

            float _smooting = m_SmoothingCurve.Evaluate((float)_progress / m_Parts.Count) * m_SmoothingMultiplyStoE;

            // ‹K’è‚æ‚è‚à‹——£‚ª—£‚ê‚Ä‚¢‚½‚ç‹ß‚Ã‚¯‚é
            var _diff = _prev.position - _part.position;
            if (m_Distance < _diff.magnitude)
            {
                var _destination = _prev.position + (_part.position - _prev.position).normalized * m_Distance;
                var _smoothDest = Vector3.Lerp(_part.position, _destination, Time.deltaTime * _smooting);
                _part.position = _smoothDest;
            }
            _prev = _part;
        }

        var _prevPos = m_ChainStart.position;
        foreach (var _part in m_Parts)
        {
            _part.rotation = Quaternion.LookRotation(_prevPos - _part.position);
            _prevPos = _part.position;
        }
    }
}
