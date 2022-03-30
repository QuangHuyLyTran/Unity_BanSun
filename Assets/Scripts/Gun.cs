using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform barrel;
    public float range = 0f;
    public float delay = 0f;
    bool fired;
    bool isAuto;
    public KeyCode switchFireMode;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletSpeed = 30;
    public float lifeTime = 3;
    public AudioSource m_audio;

    public ParticleSystem ps;                 
    private IEnumerator coroutine;

    private void Start()
    {
        ps.Clear();
        ps.Pause();
        coroutine = Wait(2.0f);
        StartCoroutine(coroutine);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {       
            m_audio.Play();
            fired = true;
            StartCoroutine("FireAuto");
            Fire();
            ps.Play();   
           
        }

        if(Input.GetKeyDown(KeyCode.Space) && !fired && isAuto)
        {
            
        }

        if (Input.GetButtonDown("Fire1") && !fired && !isAuto)
        {
            fired = true;
            StartCoroutine("FireSemi");
        }

        if (Input.GetKeyDown(switchFireMode))
        {
            if (!isAuto)
            {
                isAuto = true;
            }
            else
            {
                isAuto = false;
            }
                
        }
    }

    IEnumerator FireAuto() 
    {
        RaycastHit hit;
        Ray ray = new Ray(barrel.position, transform.forward);

        if (Physics.Raycast(ray, out hit, range)) 
        {
            if (hit.collider.tag == "Enemy") 
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                enemy.health -= 50;
            }
        }

        Debug.DrawRay(barrel.position, transform.forward * range, Color.green );
        yield return new WaitForSeconds(delay);
        fired = false;
    }

    IEnumerator FireSemi()
    {
        RaycastHit hit;
        Ray ray = new Ray(barrel.position, transform.forward);

        if (Physics.Raycast(ray, out hit, range))
        {
            if (hit.collider.tag == "Enemy")
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                enemy.health -= 50;
            }
        }

        Debug.DrawRay(barrel.position, transform.forward * range, Color.red);
        yield return null;
        fired = false;
    }
        
    private IEnumerator Wait(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            ps.Clear();
            ps.Pause();
        }
    }
private void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab);

        Physics.IgnoreCollision(bullet.GetComponent<Collider>(),
                                bulletSpawn.parent.GetComponent<Collider>());

        bullet.transform.position = bulletSpawn.position;

        var rot = bullet.transform.rotation.eulerAngles;

        bullet.transform.rotation = Quaternion.Euler(rot.x, transform.eulerAngles.y, rot.z);

        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward * bulletSpeed, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(bullet, lifeTime));

        
    }


    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(bullet);
    }

}
