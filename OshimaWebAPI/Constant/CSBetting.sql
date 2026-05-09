-- 导出  表 fungame.csbetting_bet_records 结构
CREATE TABLE IF NOT EXISTS `csbetting_bet_records` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT COMMENT '投注记录ID',
  `user_id` bigint unsigned NOT NULL COMMENT '用户UID',
  `match_id` int unsigned NOT NULL COMMENT '比赛ID',
  `option_type` tinyint NOT NULL COMMENT '投注类型：1=队伍1胜，2=队伍2胜，3=精确比分，4=赛事MVP',
  `option_value` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '投注值，如 team1, team2, 16:14, 1001',
  `amount` bigint unsigned NOT NULL COMMENT '投注金币数',
  `bet_time` datetime NOT NULL COMMENT '投注时间',
  `is_settled` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否已结算：0=未结算，1=已结算',
  `payout` bigint unsigned DEFAULT NULL COMMENT '派彩金额（含本金），NULL表示未结算',
  `is_claimed` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否已领取派彩：0=未领取，1=已领取',
  `result_note` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '结算备注，如 中奖、未中奖',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_match_id` (`match_id`),
  KEY `idx_is_settled` (`is_settled`),
  KEY `idx_bet_time` (`bet_time`),
  CONSTRAINT `fk_bet_match` FOREIGN KEY (`match_id`) REFERENCES `csbetting_matches` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='CS竞猜投注记录表';

-- 导出  表 fungame.csbetting_events 结构
CREATE TABLE IF NOT EXISTS `csbetting_events` (
  `id` int unsigned NOT NULL AUTO_INCREMENT COMMENT '赛事ID',
  `name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '赛事名称',
  `status` tinyint NOT NULL DEFAULT '0' COMMENT '赛事状态：0=未开始，1=进行中，2=已结束',
  `start_time` datetime NOT NULL COMMENT '赛事开始时间',
  `end_time` datetime NOT NULL COMMENT '赛事结束时间',
  `mvp_candidates` json DEFAULT NULL COMMENT 'MVP候选人UID列表，如 [1001, 1002]',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  PRIMARY KEY (`id`),
  KEY `idx_status` (`status`),
  KEY `idx_start_time` (`start_time`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='CS赛事表';

-- 导出  表 fungame.csbetting_matches 结构
CREATE TABLE IF NOT EXISTS `csbetting_matches` (
  `id` int unsigned NOT NULL AUTO_INCREMENT COMMENT '比赛ID',
  `event_id` int unsigned NOT NULL COMMENT '所属赛事ID',
  `stage` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '比赛阶段，如Group Stage, Quarter-final, Semi-final, Final',
  `team1_name` varchar(80) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '队伍1名称',
  `team1_logo` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '队伍1标志URL',
  `team2_name` varchar(80) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '队伍2名称',
  `team2_logo` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '队伍2标志URL',
  `status` tinyint NOT NULL DEFAULT '0' COMMENT '比赛状态：0=未开始，1=进行中，2=已结束',
  `start_time` datetime NOT NULL COMMENT '比赛开始时间',
  `bet_deadline` datetime NOT NULL COMMENT '投注截止时间',
  `result` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '比赛结果，如 16:14 或 2:1',
  `winner` tinyint DEFAULT NULL COMMENT '获胜方：1=队伍1，2=队伍2，NULL=未定',
  `available_options` json NOT NULL COMMENT '可用投注选项，如 ["team1_win","team2_win","score"]',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_event_id` (`event_id`),
  KEY `idx_status` (`status`),
  KEY `idx_bet_deadline` (`bet_deadline`),
  KEY `idx_start_time` (`start_time`),
  CONSTRAINT `fk_matches_event` FOREIGN KEY (`event_id`) REFERENCES `csbetting_events` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='CS比赛表';

-- PATCH

-- 为比赛表添加猜胜者赔率字段
ALTER TABLE `csbetting_matches`
    ADD COLUMN `team1_win_odds` DECIMAL(5,2) NOT NULL DEFAULT 2.50 COMMENT '队伍1胜赔率' AFTER `available_options`,
    ADD COLUMN `team2_win_odds` DECIMAL(5,2) NOT NULL DEFAULT 2.50 COMMENT '队伍2胜赔率' AFTER `team1_win_odds`;
