using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private AudioSource portalSound;
    public Portal linkedPortal;
    public bool isBlue = true;

    private Vector3 range;

    void Start()
    {
        portalSound.Stop();
        CreateChildrenIfMissing();

        Transform blueChild = transform.Find("PortalBlue");
        Transform orangeChild = transform.Find("PortalOrange");

        if (blueChild != null) blueChild.gameObject.SetActive(isBlue);
        if (orangeChild != null) orangeChild.gameObject.SetActive(!isBlue);

    }

    void CreateChildrenIfMissing()
    {
        if (transform.Find("PortalBlue") == null)
        {
            GameObject blue = new GameObject("PortalBlue");
            blue.transform.SetParent(transform, false);
            blue.transform.localPosition = Vector3.zero;
            blue.AddComponent<SpriteRenderer>().color = new Color(0.663f, 0.588f, 1f);
        }

        if (transform.Find("PortalOrange") == null)
        {
            GameObject orange = new GameObject("PortalOrange");
            orange.transform.SetParent(transform, false);
            orange.transform.localPosition = Vector3.zero;
            orange.AddComponent<SpriteRenderer>().color = new Color(1f, 0.68f, 0.36f);
        }
    }

    public void LinkTo(Portal otherPortal)
    {
        linkedPortal = otherPortal;
        otherPortal.linkedPortal = this;
    }

    public void Unlink()
    {
        if (linkedPortal != null)
        {
            linkedPortal.linkedPortal = null;
            linkedPortal = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (linkedPortal != null)
        {
            range = linkedPortal.transform.position - this.transform.position;
            if (other.CompareTag("Player"))
            {
<<<<<<< Updated upstream
                GhostMovement.offset = range;
=======
                GhostMovement.calc[0] = this.side;
                GhostMovement.calc[1] = this.linkedPortal.side;
                if ((this.side == Side.Left && this.linkedPortal.side == Side.Right) || (this.side == Side.Right && this.linkedPortal.side == Side.Left) || (this.side == Side.Top && this.linkedPortal.side == Side.Bottom) || (this.side == Side.Bottom && this.linkedPortal.side == Side.Top))
                {
                    GhostMovement.offset = range;
                }

                PlayerMovement.isInPortal = true;
                PlayerMovement.linked = this.linkedPortal;
>>>>>>> Stashed changes
            }
            if (other.CompareTag("PortalTrigger"))
            {
<<<<<<< Updated upstream
                other.GetComponentInParent<CapsuleCollider2D>().gameObject.transform.position += Vector3.right * 0.11f + range;
                Debug.Log($"Portal at {linkedPortal.transform.position} | Teleported at: {other.transform.position}");
=======
                if ((other.gameObject.name == "PortalTriggerLeft" && this.side == Side.Right) || (other.gameObject.name == "PortalTriggerRight" && this.side == Side.Left) || (other.gameObject.name == "PortalTriggerBott" && this.side == Side.Top) || (other.gameObject.name == "PortalTriggerTop" && this.side == Side.Bottom))
                {
                    portalSound.Play();
                    TeleportPlayer(other);
                    Debug.Log($"Portal at {this.linkedPortal.transform.position} | Teleported at: {other.transform.position}");
                    Debug.Log($"Unlock {unlock}");
                }
                else
                {
                    PlayerMovement.isInPortal = true;
                    PlayerMovement.linked = this.linkedPortal;
                }
            }



            if (!string.IsNullOrEmpty(other.tag) && other.tag.StartsWith("PortalWallTrigger"))
            {
                if (other.tag.Substring(17, 1) == "V")
                    activeVerticalTriggers.Add(other.gameObject);
                else
                {
                    activeHorizontalTriggers.Add(other.gameObject);
                    Debug.Log(other.tag);
                }


                if ((activeVerticalTriggers.Count == 2) || (activeHorizontalTriggers.Count == 2))
                {
                    Collider2D[] walscols = GetWallCollidersBySide(this.side);

                    GameObject playerObj = other.transform.parent != null
                            ? other.transform.parent.gameObject
                            : other.GetComponentInParent<Collider2D>()?.gameObject;

                    Collider2D playerCollider = playerObj != null
                                                ? playerObj.GetComponent<Collider2D>()
                                                : other.GetComponentInParent<Collider2D>();

                    foreach (Collider2D wallCol in walscols)
                    {
                        if (wallCol != null && playerCollider != null)
                        {
                            Physics2D.IgnoreCollision(playerCollider, wallCol, true);
                        }
                    }

                    Debug.Log("Wall phasing ENABLED");
                    PlayerMovement.isInPortal = true;
                    PlayerMovement.linked = this.linkedPortal;

                }
            }

            // Если это куб
            if (other.CompareTag("Cube"))
            {
                TeleportCube(other);
            }
        }
    }

    private void TeleportCube(Collider2D cubeCollider)
    {
        if (cubeCollider == null || linkedPortal == null) return;

        Cube cube = cubeCollider.GetComponent<Cube>();
        if (cube == null || cube.isPickedUp) return;

        Rigidbody2D rb = cube.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        // Вычисляем смещение для выхода из портала
        Vector3 exitOffset = Vector3.zero;
        float exitDistance = 0.5f; // Небольшое расстояние от портала

        // Определяем направление выхода в зависимости от стороны портала
        switch (linkedPortal.side)
        {
            case Side.Right:
                exitOffset = Vector3.left * exitDistance;
                break;
            case Side.Left:
                exitOffset = Vector3.right * exitDistance;
                break;
            case Side.Top:
                exitOffset = Vector3.down * exitDistance;
                break;
            case Side.Bottom:
                exitOffset = Vector3.up * exitDistance;
                break;
        }

        // Телепортируем куб на позицию выходного портала со смещением
        Vector3 newPos = linkedPortal.transform.position + exitOffset;
        rb.position = newPos;

        // Синхронизируем физику
        Physics2D.SyncTransforms();

        // Если выходной портал снизу - добавляем вертикальный импульс "выплевывания"
        if (linkedPortal.side == Side.Bottom)
        {
            // Добавляем импульс вверх
            float upwardForce = 10f; // Сила выброса
            float force = upwardForce > Math.Abs(rb.velocity.y) ? upwardForce : Math.Abs(rb.velocity.y);
            rb.velocity = new Vector2(rb.velocity.x, force);

            // Можно также добавить небольшой рандомный горизонтальный разброс
            //float minHorizontalSpread = 4f;
            //float maxHorizontalSpread = 7f;
            //float randomX = UnityEngine.Random.Range(minHorizontalSpread, maxHorizontalSpread);
            //int sign = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
            //rb.velocity = new Vector2(randomX * sign, rb.velocity.y);

            Debug.Log($"Cube spat out from bottom portal with force: {rb.velocity}");
        }

        Debug.Log($"Cube teleported from {transform.position} to {newPos} via portal");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (GhostMovement.calc[0] != Side.Default && other.name == "RealPlayer")
        {
            Vector3 range = this.linkedPortal.transform.position - this.transform.position;
            Vector3 rangePlayer = this.linkedPortal.transform.position - other.transform.position;
            if (this.side == Side.Right)
            {
                if (this.linkedPortal.side == Side.Top)
                {
                    float offset = MathF.Abs(this.transform.position.x - other.transform.position.x);

                    Vector3 adjustedOffset = new Vector3(range.x + offset, rangePlayer.y + offset);

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == Side.Bottom)
                {
                    float offset = MathF.Abs(this.transform.position.x - other.transform.position.x);

                    Vector3 adjustedOffset = new Vector3(range.x + offset, range.y - offset);

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == this.side)
                {
                    float offset = this.transform.position.x - other.transform.position.x;
                    GhostMovement.offset = new Vector3(range.x + 2f * offset, range.y);
                }
            }
            else if (this.side == Side.Left)
            {
                if (this.linkedPortal.side == Side.Top)
                {
                    float offset = MathF.Abs(this.transform.position.x - other.transform.position.x);

                    Vector3 adjustedOffset = new Vector3(range.x - offset, rangePlayer.y + offset);

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == Side.Bottom)
                {
                    float offset = MathF.Abs(this.transform.position.x - other.transform.position.x);

                    Vector3 adjustedOffset = new Vector3(range.x - offset, rangePlayer.y - offset);

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == this.side)
                {
                    float offset = this.transform.position.x - other.transform.position.x;
                    GhostMovement.offset = new Vector3(range.x + 2f * offset, range.y);
                }
            }
            else if (this.side == Side.Bottom)
            {
                if (this.linkedPortal.side == Side.Right)
                {
                    Vector3 offset = (this.transform.position - other.transform.position);

                    Vector3 adjustedOffset = new Vector3(range.x + offset.x - offset.y, range.y + offset.y);

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == Side.Left)
                {
                    Vector3 offset = this.transform.position - other.transform.position;

                    Vector3 adjustedOffset = new Vector3(range.x + offset.x - MathF.Abs(offset.y), range.y + offset.y);

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == this.side)
                {
                    Vector3 offset = this.transform.position - other.transform.position;
                    GhostMovement.offset = new Vector3(range.x + offset.x, range.y + 2f * offset.y);
                }
            }
            else if (this.side == Side.Top)
            {
                if (this.linkedPortal.side == Side.Right)
                {
                    Vector3 offset = this.transform.position - other.transform.position;

                    Vector3 adjustedOffset = new Vector3(range.x + offset.x + MathF.Abs(offset.y), range.y + MathF.Abs(offset.y));

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == Side.Left)
                {
                    Vector3 offset = this.transform.position - other.transform.position;

                    Vector3 adjustedOffset = new Vector3(range.x + offset.x - MathF.Abs(offset.y), range.y + MathF.Abs(offset.y));

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == this.side)
                {
                    Vector3 offset = this.transform.position - other.transform.position;
                    GhostMovement.offset = new Vector3(range.x + offset.x, range.y + 2f * offset.y);
                }
>>>>>>> Stashed changes
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && linkedPortal != null)
            GhostMovement.offset = Vector3.up * 25;
    }




}