using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameProcessManager processManager;
    [SerializeField] PlayerAnimationController aniController;
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerFight fighting;
    [SerializeField] PlayerUI ui;
    private GameControls controls;
    private Vector2 rightStickInput;
    private void Awake()
    {
        // 컨트롤 인스턴스 생성
        controls = new GameControls();

        // 오른쪽 스틱 입력에 콜백 함수 연결
        controls.Gameplay.RightStick.performed += ctx => rightStickInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.RightStick.canceled += ctx => rightStickInput = Vector2.zero;

    }
    void Start()
    {
        ui.PadExecution(false);
    }
    private void FixedUpdate()
    {
        if (!fighting.isStunned)
        {
            movement.Move();
            movement.Turn(fighting.canAimToLockedEnemy, fighting.aimDir);
        }
        if (fighting.executing) movement.RbStop();
    }
    private void Update()
    {
        Setting();
        CheckEnd();
        KeyControll();
        UIPadSet();
    }
    void Setting()
    {
        ui.SetAmmoText(fighting.getCurAmmo, fighting.getTotalAmmo);
        aniController.IsExecution(fighting.executing);
        aniController.MoveAni(movement.isMoving);
    }
    void CheckEnd()
    {
        if (fighting.hp <= 0)
        {
            if (ui.isPad)
                Gamepad.current.SetMotorSpeeds(0f, 0f);
            processManager.GameOver();
            fighting.DelStunObj();
        }
    }

    [SerializeField] float rsDeadZone = 0.5f;
    [SerializeField] Text rsrsrs;
    bool flag;
    void KeyControll()
    {
        //rsrsrs.text = $"RSM {rightStickInput.magnitude}";
        bool isRightStickActive = rightStickInput.magnitude > rsDeadZone;
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetButtonDown("LockOn")) // 락온 토글
        {
            print("락온켜짐");
            fighting.LockOn();
        }
        if (flag == false && (Input.GetKeyDown(KeyCode.Tab) || isRightStickActive)) // 락온 변경
        {
            flag = true;
            print("변경!");
            fighting.ChangeLockOn();
        }
        else if (!isRightStickActive || Input.GetKeyUp(KeyCode.Tab))
        {
            flag = false;
        }
        if (Input.GetKeyDown(KeyCode.I) || Input.GetButtonDown("MeleeAttack")) // 근접
        {
            fighting.ExecutionGaugeFill();
            if (fighting.knifeAble && !fighting.executing)
            {
                aniController.KnifeHit();
                fighting.KnifeAttack();
            }
        }
        if (Input.GetKey(KeyCode.J) || Input.GetAxisRaw("RangeAttack") != 0) // 원거리 키 누른거
        {

            if (!fighting.getShootable || fighting.onReloading) return;
            if (fighting.getCurAmmo > 0)
            {
                movement.Slow();
                aniController.Fire();
                if (ui.isPad)
                    TriggerVibration(1f, 1f, 0.2f);
            }
            fighting.Shoot();
        }
        if (Input.GetKeyUp(KeyCode.J) || Input.GetAxisRaw("RangeAttack") == 0) // 원거리 키 뗀거
        {
            movement.Slow(false);
        }
        if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Reload")) // 재장전
        {
            if (fighting.onReloading || !fighting.CheckReloadable()) return;
            fighting.Reloading(fighting.getReloadTime);
            ui.ReloadSlider(fighting.getReloadTime);
        }
        if ((Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("Dodge")) && !fighting.isStunned) // 대쉬
        {
            movement.Dash();
        }
        // if (Input.GetKeyDown(KeyCode.LeftShift)) // 회전 락
        // {
        // movement.StayLocked();
        // }
    }
    bool isPad;
    void UIPadSet()
    {
        if (isPad) return;

        if (Input.GetButtonDown("Dodge") ||
        Input.GetButtonDown("Reload") ||
        Input.GetAxisRaw("RangeAttack") != 0 ||
        Input.GetButtonDown("MeleeAttack") ||
         Mathf.Abs(Input.GetAxis("RightStickHorizontal")) > 0.1f ||
         Mathf.Abs(Input.GetAxis("RightStickVertical")) > 0.1f ||
         Input.GetButtonDown("LockOn")
        )
        {
            isPad = true;
            ui.PadExecution(true);
        }
    }
    public void TriggerVibration(float leftMotorSpeed, float rightMotorSpeed, float duration)
    {
        if (Gamepad.current != null)
        {
            // 진동 강도를 설정
            Gamepad.current.SetMotorSpeeds(leftMotorSpeed, rightMotorSpeed);
            // 지정된 시간 후에 진동 멈추기
            StartCoroutine(StopVibrationAfterDelay(duration));
        }
    }

    private System.Collections.IEnumerator StopVibrationAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);

        // 진동 멈추기
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "EndPoint")
        {
            processManager.GameClear();
        }
    }
    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}
