/*
 Navicat Premium Data Transfer

 Source Server         : local
 Source Server Type    : MySQL
 Source Server Version : 50724
 Source Host           : localhost:3306
 Source Schema         : accountzf

 Target Server Type    : MySQL
 Target Server Version : 50724
 File Encoding         : 65001

 Date: 30/08/2019 20:38:50
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for account
-- ----------------------------
DROP TABLE IF EXISTS `account`;
CREATE TABLE `account`  (
  `id` bigint(12) NOT NULL AUTO_INCREMENT,
  `name` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `password` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `last_login` int(4) NOT NULL DEFAULT 0,
  `lock` tinyint(1) NOT NULL DEFAULT 0,
  `vip` int(4) UNSIGNED NOT NULL DEFAULT 1,
  `type` int(4) NOT NULL DEFAULT 2,
  `mac_addr` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '000000000000',
  `lock_expire` int(4) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3810 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of account
-- ----------------------------
INSERT INTO `account` VALUES (3808, 'test', 'b913d5bbb8e461c2c5961cbe0edcdadfd29f068225ceb37da6defcf89849368f8c6c2eb6a4c4ac75775d032a0ecfdfe8550573062b653fe92fc7b8fb3b7be8d6', 1567200943, 0, 1, 4, '00D86104CF62', 0);
INSERT INTO `account` VALUES (3809, 'test2', 'b913d5bbb8e461c2c5961cbe0edcdadfd29f068225ceb37da6defcf89849368f8c6c2eb6a4c4ac75775d032a0ecfdfe8550573062b653fe92fc7b8fb3b7be8d6', 1566754004, 0, 0, 2, '00D86104CF62', 0);

-- ----------------------------
-- Table structure for login_rcd
-- ----------------------------
DROP TABLE IF EXISTS `login_rcd`;
CREATE TABLE `login_rcd`  (
  `id` int(4) UNSIGNED NOT NULL AUTO_INCREMENT,
  `account_id` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `login_time` int(4) UNSIGNED NOT NULL DEFAULT 0,
  `mac_adr` char(12) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `ip_adr` char(16) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '0',
  `res_src` char(4) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 98 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
