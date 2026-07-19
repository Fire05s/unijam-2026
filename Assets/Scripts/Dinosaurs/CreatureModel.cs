using UnityEngine;

public class CreatureModel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _anim;

    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
    }

    public void SetAttack()
    {
        _anim.SetTrigger("Attack");
    }

    public void SetHeadbutt()
    {
        _anim.SetTrigger("Headbutt");
    }

    public void SetIntro()
    {
        _anim.SetTrigger("Intro");
    }

    public void SetHurt()
    {
        _anim.SetTrigger("Hit");
    }

    public void SetDead()
    {
        _anim.SetTrigger("Death");
    }
}
