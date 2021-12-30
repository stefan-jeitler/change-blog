# UserAccess Test users

In this document you'll find account and user information that can be used for testing the **authentication** and  
**authorization** mechanism.

At the end there is a sql script that inserts the test data.  
But be aware to not execute the script against the staging or production environment.  
It is **only** for the **testing** environment.

## Prefix

All test entities starts with the prefix **`t_ua`**.  
`t` stands for test and `ua` for user access.  
This is used for grouping the testing purpose.  
User access tests covers Authentication and Authorization.

## Accounts

Two test accounts will be created, named like the following.

* `t_ua_account_01`
* `t_ua_account_02`

## Users

Usernames will start with the account name followed by `_user_<number>`.

| User                    | Email                                   | ApiKey     |
|-------------------------|-----------------------------------------|------------|
| t_ua_account_01_user_01 | t_ua_account_01_user_01@change-blog.com | acc01usr01 |
| t_ua_account_01_user_02 | t_ua_account_01_user_02@change-blog.com | acc01usr02 |
| t_ua_account_02_user_01 | t_ua_account_02_user_01@change-blog.com | acc02usr01 |
| t_ua_account_02_user_02 | t_ua_account_02_user_02@change-blog.com | acc02usr02 |
| t_ua_account_02_user_03 | t_ua_account_02_user_03@change-blog.com | acc02usr03 |

## Roles on account level

```json
[
  {
    "t_ua_account_01_user_01": [
      "DefaultUser"
    ]
  },
  {
    "t_ua_account_01_user_02": [
      "DefaultUser",
      "PlatformManager"
    ]
  },
  {
    "t_ua_account_02_user_01": [
      "DefaultUser",
      "ProductOwner"
    ]
  },
  {
    "t_ua_account_02_user_02": [
      "DefaultUser",
      "Developer",
      "ScrumMaster"
    ]
  },
  {
    "t_ua_account_02_user_03": [
      "DefaultUser",
      "Support"
    ]
  }
]
```

## Products

Products starts with the account name followed by `_proj_<number>`.

* `t_ua_account_01_proj_01`
* `t_ua_account_01_proj_02`
* `t_ua_account_02_proj_01`
* `t_ua_account_02_proj_02`

## Product Userroles

```json
[
  {
    "t_ua_account_01_proj_01": {
      "t_ua_account_01_user_01": [
        "ProductManager"
      ]
    }
  },
  {
    "t_ua_account_02_proj_02": {
      "t_ua_account_02_user_03": [
        "Developer"
      ]
    }
  }
]
```

## Insert Script

```sql
-- accounts
insert into account
values ('ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc',
        't_ua_account_01',
        null,
        null,
        now())
on conflict (id) do nothing;

insert into account
values ('93addfe7-0d71-4063-81ce-63ac2a59dc71',
        't_ua_account_02',
        null,
        null,
        now())
on conflict (id) do nothing;

-- users
insert into "user"
values ('f575503e-4eee-4d6d-b2c1-f11d8fc3da76',
        't_ua_account_01_user_01@change-blog.com',
        't_ua_account_01', 'user_01',
        'Europe/Berlin', null, now())
on conflict (id) do nothing;

insert into "user"
values ('7aa9004b-ed6f-4862-8307-579030c860be',
        't_ua_account_01_user_02@change-blog.com',
        't_ua_account_01', 'user_02',
        'Europe/Berlin', null, now())
on conflict (id) do nothing;

insert into "user"
values ('146929d6-ee37-4298-8ce2-53e44494a089',
        't_ua_account_02_user_01@change-blog.com',
        't_ua_account_02', 'user_01',
        'Europe/Berlin', null, now())
on conflict (id) do nothing;

insert into "user"
values ('79630f70-3448-41d2-929c-f8d12cf7c43a',
        't_ua_account_02_user_02@change-blog.com',
        't_ua_account_02', 'user_02',
        'Europe/Berlin', null, now())
on conflict (id) do nothing;

insert into "user"
values ('ff1dc8c5-7064-431d-9ab5-e1427cde2bb7',
        't_ua_account_02_user_03@change-blog.com',
        't_ua_account_02', 'user_03',
        'Europe/Berlin', null, now())
on conflict (id) do nothing;

insert into api_key values ('7244eea5-c619-4f86-8319-42f94e99f8e7',
                            (select id from "user" where email = 't_ua_account_01_user_01@change-blog.com'),
                            'acc01usr01', to_timestamp('31.12.9999', 'DD.MM.YYYY'), null, now())
                            on conflict (id) do nothing ;


insert into api_key values ('240e12b8-f5c4-4c53-807b-3a991c7f1b55',
                            (select id from "user" where email = 't_ua_account_01_user_02@change-blog.com'),
                            'acc01usr02', to_timestamp('31.12.9999', 'DD.MM.YYYY'), null, now())
                            on conflict (id) do nothing ;

insert into api_key values ('818a0dae-0797-4fbd-a673-857ef67b1678',
                            (select id from "user" where email = 't_ua_account_02_user_01@change-blog.com'),
                            'acc02usr01', to_timestamp('31.12.9999', 'DD.MM.YYYY'), null, now())
                            on conflict (id) do nothing ;

insert into api_key values ('2c16969f-e60b-46bb-90aa-84255661e4fe',
                            (select id from "user" where email = 't_ua_account_02_user_02@change-blog.com'),
                            'acc02usr02', to_timestamp('31.12.9999', 'DD.MM.YYYY'), null, now())
                            on conflict (id) do nothing ;

insert into api_key values ('04cd7867-d084-4abf-bbb4-d8d3a87dafc0',
                            (select id from "user" where email = 't_ua_account_02_user_03@change-blog.com'),
                            'acc02usr03', to_timestamp('31.12.9999', 'DD.MM.YYYY'), null, now())
                            on conflict (id) do nothing ;

insert into account_user values ((select id from account where name = 't_ua_account_01'),
                                 (select id from "user" where email = 't_ua_account_01_user_01@change-blog.com'),
                                 (select id from role where name = 'DefaultUser'),
                                 now()
                                )
on conflict (account_id, user_id, role_id) do nothing ;


insert into account_user values ((select id from account where name = 't_ua_account_01'),
                                 (select id from "user" where email = 't_ua_account_01_user_02@change-blog.com'),
                                 (select id from role where name = 'DefaultUser'),
                                 now()
                                )
on conflict (account_id, user_id, role_id) do nothing ;

insert into account_user values ((select id from account where name = 't_ua_account_01'),
                                 (select id from "user" where email = 't_ua_account_01_user_02@change-blog.com'),
                                 (select id from role where name = 'PlatformManager'),
                                 now()
                                )
on conflict (account_id, user_id, role_id) do nothing ;


insert into account_user values ((select id from account where name = 't_ua_account_02'),
                                 (select id from "user" where email = 't_ua_account_02_user_01@change-blog.com'),
                                 (select id from role where name = 'DefaultUser'),
                                 now()
                                )
on conflict (account_id, user_id, role_id) do nothing ;

insert into account_user values ((select id from account where name = 't_ua_account_02'),
                                 (select id from "user" where email = 't_ua_account_02_user_01@change-blog.com'),
                                 (select id from role where name = 'ProductOwner'),
                                 now()
                                )
on conflict (account_id, user_id, role_id) do nothing ;


insert into account_user values ((select id from account where name = 't_ua_account_02'),
                                 (select id from "user" where email = 't_ua_account_02_user_02@change-blog.com'),
                                 (select id from role where name = 'DefaultUser'),
                                 now()
                                )
on conflict (account_id, user_id, role_id) do nothing ;

insert into account_user values ((select id from account where name = 't_ua_account_02'),
                                 (select id from "user" where email = 't_ua_account_02_user_02@change-blog.com'),
                                 (select id from role where name = 'Developer'),
                                 now()
                                )
on conflict (account_id, user_id, role_id) do nothing ;

insert into account_user values ((select id from account where name = 't_ua_account_02'),
                                 (select id from "user" where email = 't_ua_account_02_user_02@change-blog.com'),
                                 (select id from role where name = 'ScrumMaster'),
                                 now()
                                )
on conflict (account_id, user_id, role_id) do nothing ;


insert into account_user values ((select id from account where name = 't_ua_account_02'),
                                 (select id from "user" where email = 't_ua_account_02_user_03@change-blog.com'),
                                 (select id from role where name = 'DefaultUser'),
                                 now()
                                )
on conflict (account_id, user_id, role_id) do nothing ;

insert into account_user values ((select id from account where name = 't_ua_account_02'),
                                 (select id from "user" where email = 't_ua_account_02_user_03@change-blog.com'),
                                 (select id from role where name = 'Support'),
                                 now()
                                )
on conflict (account_id, user_id, role_id) do nothing ;

-- products

insert into product values ('139a2e54-e9be-4168-98b4-2839d9b3db04',
                            (select id from account where name = 't_ua_account_01'),
                            '4091b948-9bc5-43ee-9f98-df3d27853565',
                            't_ua_account_01_proj_01',
                            'f575503e-4eee-4d6d-b2c1-f11d8fc3da76',
                            null, now(), 'en'
                           ) on conflict (id) do nothing ;

insert into product values ('0614f8d6-8895-4c74-bcbe-8a3c26076e1b',
                            (select id from account where name = 't_ua_account_01'),
                            '4091b948-9bc5-43ee-9f98-df3d27853565',
                            't_ua_account_01_proj_02',
                            'f575503e-4eee-4d6d-b2c1-f11d8fc3da76',
                            null, now(), 'en'
                           ) on conflict (id) do nothing ;


insert into product values ('04482211-eda1-4748-9818-9f74c105609c',
                            (select id from account where name = 't_ua_account_02'),
                            '4091b948-9bc5-43ee-9f98-df3d27853565',
                            't_ua_account_02_proj_01',
                            'f575503e-4eee-4d6d-b2c1-f11d8fc3da76',
                            null, now(), 'en'
                           ) on conflict (id) do nothing ;

insert into product values ('35c5df1a-079e-4b8c-87c5-09b30e52a82f',
                            (select id from account where name = 't_ua_account_02'),
                            '4091b948-9bc5-43ee-9f98-df3d27853565',
                            't_ua_account_02_proj_02',
                            'f575503e-4eee-4d6d-b2c1-f11d8fc3da76',
                            null, now(), 'en'
                           ) on conflict (id) do nothing ;

insert into product_user values (
                                 '139a2e54-e9be-4168-98b4-2839d9b3db04',
                                 (select id from "user" where email = 't_ua_account_01_user_01@change-blog.com'),
                                 (select id from role where name = 'ProductManager'),
                                 now()
                                ) on conflict (product_id, user_id, role_id) do nothing ;

insert into product_user values (
                                 '35c5df1a-079e-4b8c-87c5-09b30e52a82f',
                                 (select id from "user" where email = 't_ua_account_02_user_03@change-blog.com'),
                                 (select id from role where name = 'Developer'),
                                 now()
                                ) on conflict (product_id, user_id, role_id) do nothing ;                            
```
